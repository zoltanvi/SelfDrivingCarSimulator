using UnityEngine;

public class WallSensor : MonoBehaviour
{
	// Az autohoz tartozo erzekelok kezdopontja
	[SerializeField] private Transform rayOriginPoint;
	// Az erzekelo sugarak hossza
	private float lineLength;
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

	private int ID { get; set; }

	// Az erzekelok altal mert tavolsagokat es a fitnesst  tartalmazza
	private double[] carNeuronInputs;

	// A raycastHit-ben vannak az erzekelok adatai tarolva.
	private RaycastHit[] raycastHit;

	#region Navigator változói
	private Vector3[] points;
	private int one, two, three, four;
	private Vector3 first, second, third, fourth;
	private double firstAngle, secondAngle, thirdAngle;
	private FitnessMeter fm;
	public Transform[] waypoints;
	#endregion

	void Start()
	{
		rayCount = Master.Instance.Manager.CarSensorCount;
		lineLength = Master.Instance.Manager.CarSensorLength;
		// Beallitja az auto sorszamat
		ID = this.gameObject.GetComponent<CarController>().ID;

		// Inicializalja az erzekeloket
		raycastHit = new RaycastHit[rayCount];
		rayHolders = new GameObject[rayCount];
		// Inicializalja az erzekeloket reprezentalo vonalakat
		InitializeLines();

		if (this.gameObject.GetComponent<CarController>().IsPlayerControlled)
		{
			controlledByPlayer = true;
		}

		if (Master.Instance.Manager.Navigator)
		{
			// Inicializalja a neuralis halo inputjait
			carNeuronInputs = new double[rayCount + 4];

			fm = gameObject.GetComponent<FitnessMeter>();
			points = new Vector3[5];
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = new Vector3();
			}


			waypoints = new Transform[Master.Instance.Manager.CurrentWaypoint.transform.childCount];

			int index = 0;
			foreach (Transform wp in Master.Instance.Manager.CurrentWaypoint.transform)
			{
				waypoints[index++] = wp;
			}

		}
		else
		{
			// Inicializalja a neuralis halo inputjait
			carNeuronInputs = new double[rayCount + 1];
		}

	}


	void FixedUpdate()
	{
		// Erzekelo sugarak letrahozasa
		CreateRays(rayCount);

		// Erzekelo adatok tarolasa a carNeuronInputs tombben
		rawSensorText = "";
		for (int i = 0; i < rayCount; i++)
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
		carNeuronInputs[rayCount] = carRigidbody.velocity.magnitude;

		if (!controlledByPlayer)
		{
			// Atadja az erzekelo adatokat es az auto sebesseget a CarGameManagernek
			Master.Instance.Manager.Cars[ID].Inputs = carNeuronInputs;
		}

		if (Master.Instance.Manager.Navigator)
		{
			SetAnglesInput();
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
		LineRenderer lineRenderer;
		for (int i = 0; i < rayHolders.Length; i++)
		{
			rayHolders[i] = new GameObject("RayHolder_" + ID);
			rayHolders[i].transform.parent = Master.Instance.Manager.rayHolderRoot.transform;
			lineRenderer = rayHolders[i].AddComponent<LineRenderer>();
			lineRenderer.positionCount = 2;
			lineRenderer.numCapVertices = 5;
			lineRenderer.startWidth = 0.05f;
			lineRenderer.endWidth = 0.05f;
			lineRenderer.useWorldSpace = false;
			lineRenderer.material = lineMat;
			lineRenderer.startColor = new Color(0.8378353f, 0f, 1.0f, 0.02f);
			lineRenderer.endColor = new Color(0f, 0.8818119f, 1f, 0f);

		}
	}

	private void SetAnglesInput()
	{

		// Indexek
		one = ((fm.nextPointIndex + 1) > waypoints.Length - 1) ? 0 : (fm.nextPointIndex + 1);
		two = ((one + 1) > waypoints.Length - 1) ? 0 : (one + 1);
		three = ((two + 1) > waypoints.Length - 1) ? 0 : (two + 1);
		four = ((three + 1) > waypoints.Length - 1) ? 0 : (three + 1);

		// Pontok
		points[0] = waypoints[fm.nextPointIndex].position;
		points[1] = waypoints[one].position;
		points[2] = waypoints[two].position;
		points[3] = waypoints[three].position;
		points[4] = waypoints[four].position;

		// Vektorok
		first = points[1] - points[0];
		second = points[2] - points[1];
		third = points[3] - points[2];
		fourth = points[4] - points[3];

		// Szögek
		firstAngle = Vector3.Angle(first, second);
		secondAngle = Vector3.Angle(second, third);
		thirdAngle = Vector3.Angle(third, fourth);

		// Beírja az input tömbbe a szögeket
		Master.Instance.Manager.Cars[ID].Inputs[Master.Instance.Manager.CarSensorCount + 1] = firstAngle;
		Master.Instance.Manager.Cars[ID].Inputs[Master.Instance.Manager.CarSensorCount + 2] = secondAngle;
		Master.Instance.Manager.Cars[ID].Inputs[Master.Instance.Manager.CarSensorCount + 3] = thirdAngle;

	}

}
