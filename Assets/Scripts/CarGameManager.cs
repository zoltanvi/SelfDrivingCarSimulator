using System;
using System.Collections.Generic;
using System.Collections;
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
	int[][] carPairs;

	private static System.Random rand = new System.Random();

	private NeuralNetwork[] carNetwork;
	private double[,,,] savedCarNetwork;


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
		carNetwork = new NeuralNetwork[CarCount];



		carsAliveCount = CarCount;
		carPairs = new int[CarCount][];
		for (int i = 0; i < carPairs.Length; i++)
		{
			carPairs[i] = new int[2];
		}

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


		NewGeneration();


		CheckSlowCars();



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

			Debug.Log(carTransform.name + " crashed!");
			isAlive = false;
			carsAliveCount--;
			NeuralNetwork tmp2 = Cars[carIndex].Transform.gameObject.GetComponent<NeuralNetwork>();

			#region Neuronhalo print

			string carNNWeights = carIndex + ". car:\n";
			for (int i = 0; i < tmp2.NeuronLayers.Length; i++)
			{
				carNNWeights += (i + 1) + ". layer: \n";
				for (int k = 0; k < tmp2.NeuronLayers[i].NeuronWeights.Length; k++)
				{
					for (int j = 0; j < tmp2.NeuronLayers[i].NeuronWeights[0].Length; j++)
					{
						string tmp = string.Format("{0,10}", tmp2.NeuronLayers[i].NeuronWeights[k][j]);
						carNNWeights += tmp + "\t";
					}
					carNNWeights += "\n";
				}
				carNNWeights += "\n";
			}
			#endregion
			GameLogger.WriteData(carNNWeights);
			string dataText = "Maximum fitness: " + Cars[carIndex].Fitness + "\n\n";
			GameLogger.WriteData(dataText);

		}
	}

	// Rendezi az autókat a CarsData[] tömbben fitness érték szerint
	private void SortCarsByFitness()
	{

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
		Debug.Log("Sorbarendezett autók: ");
		foreach (var item in CarsData)
		{
			Debug.Log(item.Index + " : " + item.Fitness);
		}

	}

	// A játék indításakor logol, hogy jobban látható legyen melyik játékhoz tartozik.
	private void LogTimestamp()
	{
		DateTime localDate = DateTime.Now;
		string ts = "######################## New game ########################\n" + localDate.ToString() + "\n"
			+ "_______________________________________________________\n";
		GameLogger.WriteData(ts);
	}

	// Elmenti a neurális háló adatait a savedCarNetwork tömbbe
	private void SaveNeuronNetworks()
	{

		for (int i = 0; i < CarCount; i++)
		{
			carNetwork[i] = Cars[i].Transform.gameObject.GetComponent<NeuralNetwork>();
		}

		savedCarNetwork = new double[
			CarCount,
			carNetwork[0].NeuronLayers.Length,
			carNetwork[0].NeuronLayers[0].NeuronWeights.Length,
			carNetwork[0].NeuronLayers[0].NeuronWeights[0].Length];


		for (int i = 0; i < CarCount; i++)  // melyik autó
		{
			for (int j = 0; j < carNetwork[i].NeuronLayers.Length; j++) // melyik réteg
			{
				for (int k = 0; k < carNetwork[i].NeuronLayers[j].NeuronWeights.Length; k++) // melyik neuron
				{
					for (int l = 0; l < carNetwork[i].NeuronLayers[j].NeuronWeights[0].Length; l++) // melyik súlya
					{
						savedCarNetwork[i, j, k, l] = carNetwork[i].NeuronLayers[j].NeuronWeights[k][l];
					}
				}
			}
		}
	}



	private void RecombineAndMutate()
	{
		int rnd;
		int ind;
		int mutation;
		//int r;
		double tmp;

		for (int i = 0; i < CarCount; i++)  // melyik autó
		{
			for (int j = 0; j < carNetwork[i].NeuronLayers.Length; j++) // melyik réteg
			{
				for (int k = 0; k < carNetwork[i].NeuronLayers[j].NeuronWeights.Length; k++) // melyik neuron
				{
					for (int l = 0; l < carNetwork[i].NeuronLayers[j].NeuronWeights[0].Length; l++) // melyik súlya
					{
						// 50% eséllyel örököl bizonyos szülőt
						rnd = rand.Next(2);
						rnd = rnd < 2 ? 0 : 1;
						ind = carPairs[i][rnd];
						// 50% eséllyel mutálódik a súly
						mutation = rand.Next(2);
						if (mutation == 0)
						{
							tmp = UnityEngine.Random.Range(-0.2f, 0.2f);


							carNetwork[i].NeuronLayers[j].NeuronWeights[k][l] =
								savedCarNetwork[ind, j, k, l] + tmp;
						}
						else
						{
							carNetwork[i].NeuronLayers[j].NeuronWeights[k][l] =
								savedCarNetwork[ind, j, k, l];
						}

					}
				}
			}
		}

	}

	// Egy új generáció spawnolását végzi
	private void NewGeneration()
	{
		// Ha nincs már működő autó, akkor új spawn
		if (carsAliveCount == 0)
		{

			// Legyen idő megcsodálni az autókat a respawn előtt
			waitingTime += Time.deltaTime;
			if (waitingTime >= 0.5f)
			{

				// Elmenti az összes autó neurális hálóját
				SaveNeuronNetworks();

				// Az autók rendezése fitness szerint
				SortCarsByFitness();


				//#region kiíratás
				//Debug.Log("A top 50% autó indexei a következők: ");
				//for (int i = 0; i < CarsData.Length/2; i++)
				//{
				//	Debug.Log(CarsData[i].Index);
				//}
				//#endregion

				int fatherIndex = rand.Next(CarCount / 2);
				int randomFromBottomHalf = CarsData[rand.Next(CarCount / 2, CarCount)].Index;

				// random párokat készít (nem lesz önmagával párban senki)
				for (int i = 0; i < CarCount; i++)
				{
					carPairs[i][0] = CarsData[i / 2].Index;

					int rnd = carPairs[i][0];
					while (carPairs[i][0] == rnd)
					{
						rnd = CarsData[rand.Next(0, CarCount / 2)].Index;
					}
					carPairs[i][1] = rnd;
				}
				// Egy db a rosszabbik 50%ból származik!
				carPairs[fatherIndex][1] = randomFromBottomHalf;

				#region old pair maker
				//// random párokat készít (nem lesz önmagával párban senki)
				//for (int i = 0; i < carPairs.Length; i++)
				//{
				//	carPairs[i][0] = carSet[rand.Next(0, carSet.Length)];
				//	int rnd = carPairs[i][0];
				//	while (carPairs[i][0] == rnd)
				//	{
				//		rnd = carSet[rand.Next(0, carSet.Length)];
				//	}
				//	carPairs[i][1] = rnd;
				//}
				#endregion

				#region kiíratás
				for (int i = 0; i < carPairs.Length; i++)
				{
					string str = carPairs[i][0] + " :: " + carPairs[i][1];
					Debug.Log(str);
				}
				#endregion

				// ettől a ponttól kezdve megvannak a párok

				RecombineAndMutate();

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
	}

	// Ellenőrzi az autókat, és lefagyasztja amelyik nem elég gyors, vagy hátrafele megy
	private void CheckSlowCars()
	{
		checkingTimeLeft -= Time.deltaTime;
		if (checkingTimeLeft <= 0)
		{
			for (int i = 0; i < CarCount; i++)
			{
				if (Cars[i].Fitness >= 500.0)
				{
					Cars[i].Transform.gameObject.GetComponent<CarController>().Freeze();
					Cars[i].LastFitness = 0;
				}

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

}
