using UnityEngine;

public class WallSensor : MonoBehaviour
{
	// Az autohoz tartozo erzekelok kezdopontja
	[SerializeField] private Transform rayOriginPoint;
	// Az erzekelo sugarak hossza
	[Range(0f, 50f)]
	[SerializeField] private float lineLength = 25f;
	// Az autohoz tartozo erzekelok darabszama

	private int rayCount;
	// Az erzekelok csak ezen layeren levo object-eket erzekelik 
	[SerializeField] private string rayLayerName = "Environment";
	[SerializeField] private FitnessMeter fitnessMeter;
	[SerializeField] private Rigidbody carRigidbody;
	[SerializeField] private CarController carController;
	private bool controlledByPlayer = false;

	// Az UI panelen megjeleno szoveg itt lesz formazva
	private string rawSensorText = "";
	// Az erzekelok lathatatlanok, egy tombelem egy lathato vonalat tart
	private GameObject[] rayHolders;

	// Az auto sorszama - tobb autot managel a CarGameController osztaly
	private int carIndex;

	// Az erzekelok altal mert tavolsagokat es a fitnesst  tartalmazza
	private double[] carNeuronInputs;

	// A raycastHit-ben vannak az erzekelok adatai tarolva.
	private RaycastHit[] raycastHit;

	void Start()
	{
		rayCount = GameManager.Instance.CarsRayCount;
		// Beallitja az auto sorszamat
		carIndex = this.gameObject.GetComponent<CarController>().carStats.index;

		// Inicializalja a neuralis halo inputjait
		carNeuronInputs = new double[rayCount + 1];
		// Inicializalja az erzekeloket
		raycastHit = new RaycastHit[rayCount];
		rayHolders = new GameObject[rayCount];
		// Inicializalja az erzekeloket reprezentalo vonalakat
		InitializeLines();

		if (this.gameObject.GetComponent<CarController>().controlledByPlayer)
		{
			controlledByPlayer = true;
		}
	}


	void FixedUpdate()
	{
		// Erzekelo sugarak letrahozasa
		CreateRays(rayCount);

		// Erzekelo adatok tarolasa a carNeuronInputs tombben
		rawSensorText = "";
		for (int i = 0; i < carNeuronInputs.Length - 1; i++)
		{
			// Ha valamivel utkozik az erzekelo sugar akkor a mert adat tarolasa
			// a carNeuronInputs tombben, ellenben a vonal hosszat tarolja.
			if (null != raycastHit[i].collider)
			{
				carNeuronInputs[i] = raycastHit[i].distance;
			}
			else
			{
				carNeuronInputs[i] = lineLength;
			}

			// Erzekelo adatok formazasa.
			rawSensorText += (i + 1) + ". sensor: " +
				string.Format("{0:0.0000}", carNeuronInputs[i]) + "\n";
		}
		// A neuralis halo utolso inputja az auto sebessege
		carNeuronInputs[carNeuronInputs.Length - 1] = carRigidbody.velocity.magnitude;
		//Debug.Log(this.transform.name + " speed: " + carNeuronInputs[carNeuronInputs.Length - 1]);

		if (!controlledByPlayer)
		{
			// Atadja az erzekelo adatokat es az auto sebesseget a CarGameManagernek
			GameManager.Instance.Cars[carIndex].Inputs = carNeuronInputs;
			GameManager.Instance.Cars[carIndex].Distances = rawSensorText;
		}
		
	}

	// Letrehozza az erzekelo sugarakat.
	void CreateRays(int quantity)
	{
		// Az angleBase = a sugarak kozotti szog nagysaga.
		float angleBase = 180f / (quantity + 1);

		// Ha 5 erzekelo van, 45 fok lesz az erzekelok kozott
		if (quantity == 5)
		{
			for (int i = 0; i < quantity; i++)
			{
				angleBase = 180f / (quantity - 1);
				// A jelenlegi sugar szoge balrol jobbra szamitva.
				Quaternion lineRotation =
					Quaternion.AngleAxis((45 * i), (rayOriginPoint.up));

				// A sugar kezdo es vegpontjai.
				Vector3 rayOrigin = rayOriginPoint.position;
				Vector3 rayDirection = lineRotation * (-rayOriginPoint.right);

				Physics.Raycast(rayOrigin, rayDirection, out raycastHit[i], lineLength, LayerMask.GetMask(rayLayerName));

				// Megrajzolja a sugarat. 
				rayHolders[i].GetComponent<LineRenderer>().SetPosition(0, rayOrigin);
				rayHolders[i].GetComponent<LineRenderer>().SetPosition(1, rayOrigin + rayDirection * lineLength);

			}
		}
		else
		{
			for (int i = 1; i < (quantity + 1); i++)
			{
				// A jelenlegi sugar szoge balrol jobbra szamitva.
				Quaternion lineRotation =
					Quaternion.AngleAxis((angleBase * i), (rayOriginPoint.up));

				// A sugar kezdo es vegpontjai.
				Vector3 rayOrigin = rayOriginPoint.position;
				Vector3 rayDirection = lineRotation * (-rayOriginPoint.right);

				// Letrehozza a sugarat es eltarolja hogy hozzaernek-e a falhoz.
				Physics.Raycast(rayOrigin, rayDirection, out raycastHit[i - 1], lineLength, LayerMask.GetMask(rayLayerName));

				// Megrajzolja a sugarat. 
				rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(0, rayOrigin);
				rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(1, rayOrigin + rayDirection * lineLength);
			}
		}


	}

	// Inicializalja az erzekeloket reprezentalo vonalakat.
	private void InitializeLines()
	{
		Material lineMat = new Material(Shader.Find("Sprites/Default"));

		for (int i = 0; i < rayHolders.Length; i++)
		{
			rayHolders[i] = new GameObject();
			rayHolders[i].AddComponent<LineRenderer>();
			rayHolders[i].GetComponent<LineRenderer>().positionCount = 2;
			rayHolders[i].GetComponent<LineRenderer>().numCapVertices = 5;
			rayHolders[i].GetComponent<LineRenderer>().startWidth = 0.08f;
			rayHolders[i].GetComponent<LineRenderer>().endWidth = 0.08f;
			rayHolders[i].GetComponent<LineRenderer>().useWorldSpace = false;
			rayHolders[i].GetComponent<LineRenderer>().material = lineMat;
			rayHolders[i].GetComponent<LineRenderer>().startColor = new Color(0.0f, 1.0f, 0.0f, 0.06f);
			rayHolders[i].GetComponent<LineRenderer>().endColor = new Color(1.0f, 0.705f, 0.0f, 0.06f);

		}
	}

}
