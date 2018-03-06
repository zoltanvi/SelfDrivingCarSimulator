using UnityEngine;

public class CarGameManager : MonoBehaviour {

	public static CarGameManager Instance { get; private set; }

	// variables
	[Range(0, 30)]
	[SerializeField] private int numberOfCars = 1;
	[SerializeField] private GameObject carPreFab;
	[SerializeField] private FollowCar cameraFollowCar;
	[SerializeField] private GameObject myUI;
	[HideInInspector] public string[] carDistances;
	[HideInInspector] public double[] carFitness;
	[HideInInspector] public int carIndexD = 0;
	[HideInInspector] public int carIndexF = 0;
	private UIPrinter myUIPrinter;
	private Transform bestCar;
	private Transform[] cars;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		carDistances = new string[numberOfCars];
		carFitness = new double[numberOfCars];

		cars = new Transform[numberOfCars];

		myUI = GameObject.Find("myUI");
		myUIPrinter = myUI.GetComponent<UIPrinter>();


		for (int i = 0; i < numberOfCars; i++)
		{
			cars[i] = Instantiate(carPreFab, transform.position, transform.rotation).transform;
		}

		cameraFollowCar.targetCar = cars[0];
	}

	void Update()
	{
		int bestCarIndex = BestCarIndex();
		cameraFollowCar.targetCar = cars[bestCarIndex];
		myUIPrinter.SensorDistances = carDistances[bestCarIndex];
		myUIPrinter.FitnessValue = carFitness[bestCarIndex];

	}

	private int BestCarIndex()
	{
		int index = 0;
		double bestFitness = 0;
		for (int i = 0; i < numberOfCars; i++)
		{
			if (bestFitness < carFitness[i])
			{
				bestFitness = carFitness[i];
				index = i;
			}
		}
		return index;
	}
}
