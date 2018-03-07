using UnityEngine;

public class CarGameManager : MonoBehaviour {

	// Singleton
	public static CarGameManager Instance { get; private set; }

	// Valtozok
	[Range(0, 100)]
	// Az autok darabszama
	[SerializeField] private int numberOfCars = 1;
	// Az auto prefab-ja
	[SerializeField] private GameObject carPreFab;
	// Az autot koveto kamera scriptje
	[SerializeField] private FollowCar cameraFollowCar;
	// A megjeleno UI panel
	[SerializeField] private GameObject myUI;
	// Itt vannak eltarolva az osszes auto erzekeloi altal mert tavolsagok (UI szamara)
	[HideInInspector] public string[] carDistances;
	// Ebben a tombben vannak eltarolva az osszes auto fitness erteke
	[HideInInspector] public double[] carFitness;
	// A carDistances tomb indexeloje
	[HideInInspector] public int carIndexD = 0;
	// A carFitness tomb indexeloje
	[HideInInspector] public int carIndexF = 0;
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
		// tombok inicializalasa
		carDistances = new string[numberOfCars];
		carFitness = new double[numberOfCars];
		cars = new Transform[numberOfCars];

		// UI panel / script inicializalasa
		myUI = GameObject.Find("myUI");
		myUIPrinter = myUI.GetComponent<UIPrinter>();

		// Peldanyosit megadott darabszamu autot es azokat a cars[] tombbe helyezi
		for (int i = 0; i < numberOfCars; i++)
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
		myUIPrinter.FitnessValue = carFitness[bestCarIndex];
		
	}

	// Visszaadja a legmagasabb fitnessel rendelkezo auto indexet
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
