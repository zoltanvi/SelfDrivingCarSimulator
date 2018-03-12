using UnityEngine;

public class CarGameManager : MonoBehaviour
{

	// Singleton
	public static CarGameManager Instance { get; private set; }

	// Valtozok
	[Range(0, 100)]
	//[SerializeField] private int numberOfCars = 1;
	[SerializeField] private GameObject carPreFab;
	[SerializeField] private FollowCar cameraFollowCar;
	[SerializeField] private GameObject myUI;

	[Space]

	[Header("Neural network settings")]
	[Range(1, 100)]
	public int CarCount = 1;
    [Range(1, 20)]
	public int NeuronPerLayerCount = 4;
	public double[] AllCarFitness;
	public double[][] AllCarInputs;
	[Range(0, 15)]
	public int HiddenLayerCount = 2;
    [Range(1, 15)]
	public int CarsRayCount = 3;
    public double Bias = 1.0;

	// Itt vannak eltarolva az osszes auto erzekeloi altal mert tavolsagok (UI szamara)
	[HideInInspector] public string[] carDistances;

	[HideInInspector] public int CarIndexD = 0;
	[HideInInspector] public int carIndexF = 0;
	[HideInInspector] public int CarIndexN = 0;


	// A UI panel printer scriptje
	private UIPrinter myUIPrinter;
	// A jelenlegi legnagyobb fitnessel rendelkezo auto
	private Transform bestCar;
	// Az osszes auto
	private Transform[] cars;

	// Az osztalynak csak egyetlen peldanya letezhet
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
		AllCarFitness = new double[CarCount];
		AllCarInputs = new double[CarCount][];
        for (int i = 0; i < AllCarInputs.Length; i++)
        {
            // az inputok a sugarak + az autó sebessége
            AllCarInputs[i] = new double[CarsRayCount + 1];
        }

		// tombok inicializalasa
		carDistances = new string[CarCount];
		cars = new Transform[CarCount];

		// UI panel / script inicializalasa
		myUI = GameObject.Find("myUI");
		myUIPrinter = myUI.GetComponent<UIPrinter>();

		// Peldanyosit megadott darabszamu autot es azokat a cars[] tombbe helyezi
		for (int i = 0; i < CarCount; i++)
		{
			cars[i] = Instantiate(carPreFab, transform.position, transform.rotation).transform;
		}
		// Kezdetben a kamera a legelso autot koveti
		cameraFollowCar.targetCar = cars[0];
	}


	void Update()
	{
		// Lekeri a legmagasabb fitnessel rendelkezo auto fitnesset
		// a kamera ezt az autot fogja kovetni
		// az UI panelen a hozza tartozo adatok fognak megjelenni.
		int bestCarIndex = BestCarIndex();
		cameraFollowCar.targetCar = cars[bestCarIndex];
		myUIPrinter.SensorDistances = carDistances[bestCarIndex];
		myUIPrinter.FitnessValue = AllCarFitness[bestCarIndex];

	}

	// Visszaadja a legmagasabb fitnessel rendelkezo auto indexet
	private int BestCarIndex()
	{
		int index = 0;
		double bestFitness = 0;
		for (int i = 0; i < CarCount; i++)
		{
			if (bestFitness < AllCarFitness[i])
			{
				bestFitness = AllCarFitness[i];
				index = i;
			}
		}
		return index;
	}

}
