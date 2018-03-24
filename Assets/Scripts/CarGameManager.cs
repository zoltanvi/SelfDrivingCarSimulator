using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Az autóhoz tartozó adatokat tárolja
public class Car
{
	public int Index { get; set; }
	public double Fitness { get; set; }
	public double LastFitness { get; set; }
	public double[] Inputs { get; set; }
	public string Distances { get; set; }
	public string NeuralNetworkText { get; set; }
	public Transform Transform { get; set; }
}

public class IndexFitness
{
	public int Index { get; set; }
	public double Fitness { get; set; }
}

public class CarGameManager : MonoBehaviour
{

	public static CarGameManager Instance { get; private set; }

	[SerializeField] private GameObject carPreFab;
	[SerializeField] private FollowCar cameraFollowCar;
	[SerializeField] private GameObject myUI;
	[Header("Do you want to control a car?")]
	public bool manualControl;

	public float timeLeft = 10.0f;
	public const float timeOut = 7.0f;
	public float checkingTimeLeft = timeOut;
	public int carsAliveCount;
	float waitingTime = 0;
	int bestCarIndex;

	// Jelzi, hogy első indulása-e az autónak
	private bool firstStart = true;

	[Space]

	#region Neural network settings
	[Header("Neural network settings")]
	[Range(1, 100)]
	public int CarCount = 1;
	[Range(1, 20)]
	public int NeuronPerLayerCount = 4;
	[Range(0, 15)]
	public int HiddenLayerCount = 2;
	[Range(1, 15)]
	public int CarsRayCount = 3;
	public double Bias = 1.0;
	#endregion


	private UIPrinter myUIPrinter;
	private Transform bestCar;

	public Car[] Cars;
	public IndexFitness[] CarsData;

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
		carsAliveCount = CarCount;

		// Az autók adatait tároló tömb inicializálása
		CarsData = new IndexFitness[CarCount];
		Cars = new Car[CarCount];
		for (int i = 0; i < CarCount; i++)
		{
			Cars[i] = new Car
			{
				Index = i,
				Inputs = new double[CarsRayCount + 1],
				LastFitness = 0
			};
			CarsData[i] = new IndexFitness();

		}

		// UI panel / script inicializálása
		myUI = GameObject.Find("myUI");
		myUIPrinter = myUI.GetComponent<UIPrinter>();


		// Az autókat egy Queue-ban tárolja, így újra fel lehet használni azokat
		pool = new Queue<GameObject>();

		// Az autók inicializálása. Ezek még nem aktív autók!
		for (int i = 0; i < CarCount; i++)
		{
			GameObject obj = Instantiate(carPreFab, transform.position, transform.rotation);
			obj.GetComponent<CarController>().carStats.index = i;

			obj.SetActive(false);
			pool.Enqueue(obj);

			Cars[i].Transform = obj.transform;
			Cars[i].Transform.name = "Car " + (i + 1);
		}

		// A kamera kezdetben a legelső autót követi
		cameraFollowCar.targetCar = Cars[0].Transform;


		// Az autók spawnolása
		for (int i = 0; i < CarCount; i++)
		{
			SpawnFromPool(transform.position, transform.rotation);
		}

		// Első spawnolás megtörtént
		firstStart = false;

	}

	// Az inicializált autók spawnolása a kezdőpozícióba.
	// Az autók pozíció adatait stb. visszaállítja defaultra.
	public GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
	{
		GameObject objectToSpawn = pool.Dequeue();
		objectToSpawn.GetComponent<CarController>().carStats.isAlive = true;
		Rigidbody objectRigidbody = objectToSpawn.GetComponent<Rigidbody>();
		objectRigidbody.isKinematic = false;
		objectRigidbody.velocity = new Vector3(0, 0, 0);
		objectRigidbody.angularVelocity = new Vector3(0, 0, 0);
		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;

		// Ha már volt első spawnolás, akkor az autó fitness értékeinek visszaállítása.
		// (Errort dob ha még nem volt első spawnolás, ezért kell a feltétel)
		if (!firstStart)
		{
			objectToSpawn.GetComponent<FitnessMeter>().Reset();
		}

		pool.Enqueue(objectToSpawn);
		return objectToSpawn;
	}


	void Update()
	{

		// Ha nem akar vezetni a player egy autót, akkor
		// lekéri a legmagasabb fitnessel rendelkező autó fitnessét
		// és a kamera ezt az autót fogja követni,
		// az UI panelen a hozzá tartozó adatok fognak megjelenni.
		if (!manualControl)
		{
			bestCarIndex = BestCarIndex();
		}
		else
		{
			bestCarIndex = 0;
		}
		cameraFollowCar.targetCar = Cars[bestCarIndex].Transform;
		myUIPrinter.SensorDistances = Cars[bestCarIndex].Distances;
		myUIPrinter.FitnessValue = Cars[bestCarIndex].Fitness;


		// Ha nincs már működő autó, akkor új spawn
		if (carsAliveCount == 0)
		{

			// Legyen idő megcsodálni az autókat a respawn előtt
			waitingTime += Time.deltaTime;
			if (waitingTime >= 4.0f)
			{
				SaveNeuronNetworks();
				
				// Az autók rendezése fitness szerint
				SortCarsByFitness();

				for (int i = 0; i < CarsData.Length / 2; i++)
				{
					string str = CarsData[i].Index + ": " + CarsData[i].Fitness;
					Debug.Log(str);
				}



				// Autók respawnolása
				for (int i = 0; i < CarCount; i++)
				{
					SpawnFromPool(transform.position, transform.rotation);
				}

				waitingTime = 0;
				carsAliveCount = CarCount;
				checkingTimeLeft = timeOut;
			}
		}



		checkingTimeLeft -= Time.deltaTime;
		if (checkingTimeLeft <= 0)
		{
			for (int i = 0; i < CarCount; i++)
			{
				// Ha nem nőtt 5 másodperc alatt az autó fitness értéke
				// legalább 7-tel, akkor lefagyasztja az autót
				if (Cars[i].Fitness > Cars[i].LastFitness + 7)
				{
					Cars[i].LastFitness = Cars[i].Fitness;
				}
				else
				{
					Cars[i].Transform.gameObject.GetComponent<CarController>().Freeze();
					Cars[i].LastFitness = 0;
				}

			}
			checkingTimeLeft = timeOut;
		}

	}

	// Visszaadja a legmagasabb fitnessel rendelkező autó indexét
	private int BestCarIndex()
	{
		int index = 0;
		double bestFitness = 0;
		for (int i = 0; i < CarCount; i++)
		{
			if (bestFitness < Cars[i].Fitness)
			{
				bestFitness = Cars[i].Fitness;
				index = Cars[i].Index;
			}
		}
		return index;
	}

	// Lefagyasztja az autót, logolja az autóhoz tartozó neurális háló súlyait,
	// az autó fitnessét.
	public void FreezeCar(Rigidbody carRigidbody, int carIndex, Transform carTransform, ref bool isAlive)
	{
		if (isAlive)
		{
			carRigidbody.isKinematic = true;
			string dataText = Cars[carIndex].NeuralNetworkText += "Maximum fitness: " + Cars[carIndex].Fitness + "\n\n";
			GameLogger.WriteData(dataText);
			Debug.Log(carTransform.name + " crashed!");
			isAlive = false;
			carsAliveCount--;
		}
	}

	private void SortCarsByFitness()
	{
		#region Komment
		//Debug.Log("########################BEFORE SORTING#############################");

		//for (int i = 0; i < CarCount; i++)
		//{
		//	Debug.Log(Cars[i].Transform.name + " fitness: " + Cars[i].Fitness);
		//}
		#endregion

		for (int i = 0; i < CarCount; i++)
		{
			CarsData[i].Index = Cars[i].Index;
			CarsData[i].Fitness = Cars[i].Fitness;
		}

		int val = CarCount - 1;

		if ((CarCount - 1) != 0)
		{
			for (; val >= 0; val--)
			{
				for (int a = 0, b = a + 1; b <= val; a++, b++)
				{
					if (CarsData[a].Fitness < CarsData[b].Fitness)
					{
						IndexFitness tmp = CarsData[a];
						CarsData[a] = CarsData[b];
						CarsData[b] = tmp;
					}
				}
			}
		}

		#region Komment
		//Debug.Log("##########################AFTER SORTING###########################");
		//for (int i = 0; i < CarCount; i++)
		//{
		//	Debug.Log(Cars[i].Transform.name + " fitness: " + Cars[i].Fitness);
		//}
		#endregion

	}


	// A játék indításakor logol, hogy jobban látható legyen melyik játékhoz tartozik.
	private void LogTimestamp()
	{
		DateTime localDate = DateTime.Now;
		string ts = "######################## New game ########################\n" + localDate.ToString() + "\n"
			+ "_______________________________________________________\n";
		GameLogger.WriteData(ts);
	}

	private void SaveNeuronNetworks()
	{

		using (StreamWriter file = new StreamWriter(@"Assets/Resources/NetworkData.csv", false))
		{
			for (int i = 0; i < CarCount; i++)
			{
				NeuronLayer[] neuronLayers = Cars[i].Transform.gameObject.GetComponent<NeuralNetwork>().NeuronLayers;
				int carIndex = Cars[i].Index;
				double carFitness = Cars[i].Fitness;
				file.WriteLine("{0};{1}", carIndex, carFitness);
				for (int j = 0; j < neuronLayers.Length; j++)
				{
					for (int k = 0; k < neuronLayers[j].NeuronWeights.Length; k++)
					{
						for (int l = 0; l < neuronLayers[j].NeuronWeights[0].Length; l++)
						{
							if (l < neuronLayers[j].NeuronWeights[0].Length - 1)
							{
								file.Write("{0};", neuronLayers[j].NeuronWeights[k][l]);
							}
							else
							{
								file.Write("{0}", neuronLayers[j].NeuronWeights[k][l]);
							}
						}
						file.Write(file.NewLine);
					}
				}

			}
		}


	}


}
