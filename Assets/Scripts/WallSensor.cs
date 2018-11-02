using UnityEngine;


public class WallSensor : MonoBehaviour
{
	// Az autohoz tartozo erzekelok kezdopontja
	[SerializeField] private Transform rayOriginPoint;
	private float lineLength;
	private int rayCount;

	// Az erzekelok csak ezen layeren levo object-eket erzekelik 
	[SerializeField] private const string RayLayerName = "Environment";
	[SerializeField] private FitnessMeter fitnessMeter;
	[SerializeField] private Rigidbody carRigidbody;
	[SerializeField] private CarController carController;
	private bool controlledByPlayer = false;

	// Az erzekelok lathatatlanok, egy tombelem egy lathato vonalat tart
	private GameObject[] rayHolders;

	private int Id { get; set; }

	// Az erzekelok altal mert tavolsagokat es a fitnesst  tartalmazza
	private float[] carNeuronInputs;

	// A raycastHit-ben vannak az erzekelok adatai tarolva.
	private RaycastHit[] raycastHit;

	#region Navigator változói
	private Vector3[] points;
	private int one, two, three, four;
	private Vector3 first, second, third, fourth;
	private float firstAngle, secondAngle, thirdAngle;
	private FitnessMeter fm;
	public Transform[] Waypoints;
	#endregion

	private void Start()
	{
		rayCount = Master.Instance.Manager.CarSensorCount;
		lineLength = Master.Instance.Manager.CarSensorLength;
		// Beallitja az auto sorszamat
		Id = this.gameObject.GetComponent<CarController>().Id;

		// Inicializalja az erzekeloket
		raycastHit = new RaycastHit[rayCount];
		rayHolders = new GameObject[rayCount];
		// Inicializalja az erzekeloket reprezentalo vonalakat
		InitializeLines();

		if (gameObject.GetComponent<CarController>().IsPlayerControlled)
		{
			controlledByPlayer = true;
		}

		if (Master.Instance.Manager.Navigator)
		{
			// Inicializalja a neuralis halo inputjait
			// carNeuronInputs = new double[rayCount + 4];
			carNeuronInputs = new float[rayCount + 4];

			fm = gameObject.GetComponent<FitnessMeter>();
			points = new Vector3[5];
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = new Vector3();
			}


			Waypoints = new Transform[Master.Instance.Manager.CurrentWayPoint.transform.childCount];

			int index = 0;
			foreach (Transform wp in Master.Instance.Manager.CurrentWayPoint.transform)
			{
				Waypoints[index++] = wp;
			}

		}
		else
		{
			// Inicializalja a neuralis halo inputjait
			carNeuronInputs = new float[rayCount + 1];
		}

	}


	private void FixedUpdate()
	{
		// Erzekelo sugarak letrahozasa
		CreateRays(rayCount);

		// Erzekelo adatok tarolasa a carNeuronInputs tombben
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

		}
		// A neuralis halo utolso inputja az auto sebessege
		carNeuronInputs[rayCount] = carRigidbody.velocity.magnitude;

		if (!controlledByPlayer)
		{
			// Atadja az erzekelo adatokat es az auto sebesseget a CarGameManagernek
			Master.Instance.Manager.Cars[Id].Inputs = carNeuronInputs;
		}

		if (Master.Instance.Manager.Navigator)
		{
			SetAnglesInput();
		}

	}

	// Letrehozza az erzekelo sugarakat.
	private void CreateRays(int quantity)
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

				Physics.Raycast(rayOrigin, rayDirection, out raycastHit[i], lineLength, LayerMask.GetMask(RayLayerName));

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
				Physics.Raycast(rayOrigin, rayDirection, out raycastHit[i - 1], lineLength, LayerMask.GetMask(RayLayerName));

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
			rayHolders[i] = new GameObject("RayHolder_" + Id);
			rayHolders[i].transform.parent = Master.Instance.Manager.RayHolderRoot.transform;
			LineRenderer lineRenderer = rayHolders[i].AddComponent<LineRenderer>();
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
		one = ((fm.NextPointIndex + 1) > Waypoints.Length - 1) ? 0 : (fm.NextPointIndex + 1);
		two = ((one + 1) > Waypoints.Length - 1) ? 0 : (one + 1);
		three = ((two + 1) > Waypoints.Length - 1) ? 0 : (two + 1);
		four = ((three + 1) > Waypoints.Length - 1) ? 0 : (three + 1);

		// Pontok
		points[0] = Waypoints[fm.NextPointIndex].position;
		points[1] = Waypoints[one].position;
		points[2] = Waypoints[two].position;
		points[3] = Waypoints[three].position;
		points[4] = Waypoints[four].position;

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
		Master.Instance.Manager.Cars[Id].Inputs[Master.Instance.Manager.CarSensorCount + 1] = firstAngle;
		Master.Instance.Manager.Cars[Id].Inputs[Master.Instance.Manager.CarSensorCount + 2] = secondAngle;
		Master.Instance.Manager.Cars[Id].Inputs[Master.Instance.Manager.CarSensorCount + 3] = thirdAngle;

	}

}

