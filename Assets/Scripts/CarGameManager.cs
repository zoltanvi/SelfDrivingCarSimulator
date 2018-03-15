using System;
using System.Collections.Generic;
using UnityEngine;

public class CarGameManager : MonoBehaviour
{

	public static CarGameManager Instance { get; private set; }

	// Valtozok
	[SerializeField] private GameObject carPreFab;
	[SerializeField] private FollowCar cameraFollowCar;
	[SerializeField] private GameObject myUI;
	[Header("Do you want to control a car?")]
	public bool manualControl;

	[HideInInspector] public float timeLeft = 10.0f;

	[Space]

	#region Neural network settings variables
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
	private int roundCounter = 0;

	// Itt vannak eltarolva az osszes auto erzekeloi altal mert tavolsagok (UI szamara)
	[HideInInspector] public string[] carDistances;
	[HideInInspector] public string[] carNNWeights;
	#endregion

	// A UI panel printer scriptje
	private UIPrinter myUIPrinter;
	// A jelenlegi legnagyobb fitnessel rendelkezo auto
	private Transform bestCar;
	// Az osszes auto
	public Transform[] cars;

	public Queue<GameObject> pool;

	#region Singleton
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
	#endregion

	void Start()
	{
		LogTimestamp();

		AllCarFitness = new double[CarCount];
		AllCarInputs = new double[CarCount][];
		for (int i = 0; i < AllCarInputs.Length; i++)
		{
			// az inputok a sugarak + az autó sebessége
			AllCarInputs[i] = new double[CarsRayCount + 1];
		}

		// tombok inicializalasa
		carDistances = new string[CarCount];
		carNNWeights = new string[CarCount];
		cars = new Transform[CarCount];

		// UI panel / script inicializalasa
		myUI = GameObject.Find("myUI");
		myUIPrinter = myUI.GetComponent<UIPrinter>();


		// Instantiate Cars
		pool = new Queue<GameObject>();
		for (int i = 0; i < CarCount; i++)
		{
			GameObject obj = Instantiate(carPreFab, transform.position, transform.rotation);
			obj.GetComponent<CarController>().carStats.index = i;

			obj.SetActive(false);
			pool.Enqueue(obj);

			cars[i] = obj.transform;
			cars[i].name = "Car " + (i + 1);
		}

		cameraFollowCar.targetCar = cars[0];

		for (int i = 0; i < CarCount; i++)
		{
			SpawnFromPool(transform.position, transform.rotation);
		}


	}

	public GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
	{
		GameObject objectToSpawn = pool.Dequeue();
		objectToSpawn.GetComponent<CarController>().carStats.isAlive = true;
		Rigidbody objectRigidbody = objectToSpawn.GetComponent<Rigidbody>();
		objectRigidbody.isKinematic = false;
		objectRigidbody.velocity = new Vector3(0, 0, 0);
		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position + new Vector3(0, 10, 0);
		objectToSpawn.transform.rotation = rotation;

		if (roundCounter > 0)
		{
			objectToSpawn.GetComponent<FitnessMeter>().Reset();
		}
		roundCounter++;

		pool.Enqueue(objectToSpawn);
		return objectToSpawn;
	}


	void Update()
	{
		//	Debug.Log(cars[0].gameObject.GetComponent<CarController>().carStats.isAlive);
		Debug.Log(timeLeft);
		timeLeft -= Time.deltaTime;

		if (timeLeft < 0)
		{
			timeLeft = 10.0f;

			for (int i = 0; i < CarCount; i++)
			{
				SpawnFromPool(transform.position, transform.rotation);
			}
		}


		// Ha nem akar vezetni a player egy autot, akkor
		// lekeri a legmagasabb fitnessel rendelkezo auto fitnesset
		// a kamera ezt az autot fogja kovetni
		// az UI panelen a hozza tartozo adatok fognak megjelenni.
		int bestCarIndex;

		if (!manualControl)
		{
			bestCarIndex = BestCarIndex();
		}
		else
		{
			bestCarIndex = 0;
		}
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

	public void StopCar(Rigidbody carRigidbody, int carIndex, Transform carTransform, ref bool isAlive)
	{
		if (isAlive)
		{
			carRigidbody.isKinematic = true;
			carNNWeights[carIndex] += "Maximum fitness: " + AllCarFitness[carIndex] + "\n\n";
			GameLogger.WriteData(carNNWeights[carIndex]);
			Debug.Log(carTransform.name + " crashed!");
			isAlive = false;
		}
	}

	private void LogTimestamp()
	{
		DateTime localDate = DateTime.Now;
		string ts = "### New game ###\n" + localDate.ToString() + "\n"
			+ "_______________________________________________________\n";
		GameLogger.WriteData(ts);
	}

}
