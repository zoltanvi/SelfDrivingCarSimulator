using System.Collections;
using System.Collections.Generic;
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

// Index - fitness párokat tárol
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
	//[SerializeField] private GraphMaker graphMaker;

	[Header("Do you want to control a car?")]
	public bool manualControl = false;

	private const float timeOut = 10.0f;
	public float freezeTimeLeft = timeOut;
	public const float globalTimeOut = 40.0f;
	public float globalTimeLeft = globalTimeOut;

	public int carsAliveCount;
	float waitingTime = 0;
	public int bestCarIndex;
	int[][] carPairs;
	public List<double> avgFitness = new List<double>();
	public List<double> medianFitness = new List<double>();

	public int GenerationCount = 0;

	// Az autók neurális hálózataira való hivatkozás
	private NeuralNetwork[] carNetworks;

	// 4D tömbben tárolja az összes autó neurális hálójának értékeit,
	// mert rekombinációkor az eredeti értékekkel kell dolgozni.
	private double[,,,] savedCarNetwork;


	// Jelzi, hogy első kör-e (később valószínűleg egy számlálóra fogom cserélni)
	private bool firstStart = true;

	[Space]
	#region Neural network settings
	[Header("Neural network settings")]
	[Range(1, 100)]
	public int CarCount = 20;
	[Range(1, 20)]
	public int NeuronPerLayerCount = 6;
	[Range(0, 15)]
	public int HiddenLayerCount = 2;
	[Range(1, 15)]
	public int CarsRayCount = 5;
	[Range(0, 50)]
	public float MutationRate = 5;
	// Pl. Ha a mutáció ráta = 5%,
	// akkor az eredeti érték minimum 95%-a, maximum 105%-a lehet a mutálódott érték.
	private float mutationRatePercent;
	public double Bias = 1.0;
	#endregion

	private UIPrinter myUIPrinter;
	private Transform bestCar;

	// Az autókat tároló tömb
	public Car[] Cars;
	// Az autók index-fitness párjait tároló tömb
	private IndexFitness[] indexFitness;
	// Az autók újra felhasználhatók, ezért egy poolban inicializálódnak
	private Queue<GameObject> pool;

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
		carNetworks = new NeuralNetwork[CarCount];
		carsAliveCount = CarCount;

		carPairs = new int[CarCount][];
		for (int i = 0; i < carPairs.Length; i++)
		{
			carPairs[i] = new int[2];
		}

		indexFitness = new IndexFitness[CarCount];
		Cars = new Car[CarCount];
		for (int i = 0; i < CarCount; i++)
		{
			Cars[i] = new Car
			{
				Index = i,
				Inputs = new double[CarsRayCount + 1],
				LastFitness = 0
			};
			indexFitness[i] = new IndexFitness();

		}

		mutationRatePercent = MutationRate / 100;

		// UI panel / script inicializálása
		myUI = GameObject.Find("myUI");
		myUIPrinter = myUI.GetComponent<UIPrinter>();


		// Az autókat egy pool-ban tárolja, így újra fel lehet használni azokat
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
		// (Errort dobna ha első spawnoláskor elérné ezt a kódot!)
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
			bestCarIndex = GetBestLivingCarIndex();
		}
		else
		{
			bestCarIndex = 0;
		}

		cameraFollowCar.targetCar = Cars[bestCarIndex].Transform;
		myUIPrinter.FitnessValue = Cars[bestCarIndex].Fitness;


		// Új generáció létrehozása
		CreateNewGeneration();

		// Autók megfagyasztása, amikor szükséges
		FreezeSlowCars();

	}



	// Elmenti a neurális háló adatait a savedCarNetwork tömbbe
	private void SaveNeuralNetworks()
	{

		for (int i = 0; i < CarCount; i++)
		{
			carNetworks[i] = Cars[i].Transform.gameObject.GetComponent<NeuralNetwork>();
		}

		savedCarNetwork = new double[
			CarCount,
			carNetworks[0].NeuronLayers.Length,
			carNetworks[0].NeuronLayers[0].NeuronWeights.Length,
			carNetworks[0].NeuronLayers[0].NeuronWeights[0].Length];


		for (int i = 0; i < CarCount; i++)  // melyik autó
		{
			for (int j = 0; j < carNetworks[i].NeuronLayers.Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < carNetworks[i].NeuronLayers[j].NeuronWeights.Length; k++) // melyik neuron
				{
					for (int l = 0; l < carNetworks[i].NeuronLayers[j].NeuronWeights[0].Length; l++) // melyik súlya
					{
						savedCarNetwork[i, j, k, l] = carNetworks[i].NeuronLayers[j].NeuronWeights[k][l];
					}
				}
			}
		}
	}



	// Visszaadja a legmagasabb fitnessel rendelkező autó indexét, ami még életben van!
	private int GetBestLivingCarIndex()
	{
		int index = 0;

		SortCarsByFitness();

		for (int i = CarCount - 1; i >= 0; i--)
		{
			if (Cars[indexFitness[i].Index].Transform.gameObject.GetComponent<CarController>().carStats.isAlive)
			{
				index = indexFitness[i].Index;
			}
		}
		return index;
	}

	private int GetBestCarIndex()
	{
		int bestIndex = 0;
		double highestFitness = double.MinValue;
		foreach (var item in indexFitness)
		{
			if (item.Fitness > highestFitness)
			{
				highestFitness = item.Fitness;
				bestIndex = item.Index;
			}
		}
		return bestIndex;
	}


	// Lefagyasztja az autót, majd logolja az autóhoz tartozó neurális háló súlyait + az autó fitnessét.
	public void FreezeCar(Rigidbody carRigidbody, int carIndex, Transform carTransform, ref bool isAlive)
	{
		if (isAlive)
		{
			carRigidbody.isKinematic = true;


			// Debug.Log(carTransform.name + " crashed!");

			isAlive = false;
			carsAliveCount--;
			NeuralNetwork tmp2 = Cars[carIndex].Transform.gameObject.GetComponent<NeuralNetwork>();


			#region Súlyok és fitness értékek fájlba írása
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


			GameLogger.WriteData(carNNWeights);
			string dataText = "Maximum fitness: " + Cars[carIndex].Fitness + "\n\n";
			GameLogger.WriteData(dataText);
			#endregion


		}
	}



	// Rendezi az autókat az indexFitness[] tömbben fitness érték szerint
	// TODO: buborékrendezés helyett valami gyorsabb
	private void SortCarsByFitness()
	{

		for (int i = 0; i < CarCount; i++)
		{
			indexFitness[i].Index = Cars[i].Index;
			indexFitness[i].Fitness = Cars[i].Fitness;
		}

		int val = CarCount - 1;

		if ((CarCount - 1) != 0)
		{
			for (; val >= 0; val--)
			{
				for (int a = 0, b = a + 1; b <= val; a++, b++)
				{
					if (indexFitness[a].Fitness < indexFitness[b].Fitness)
					{
						IndexFitness tmp = indexFitness[a];
						indexFitness[a] = indexFitness[b];
						indexFitness[b] = tmp;
					}
				}
			}
		}
		//Debug.Log("Sorbarendezett autók: ");
		//foreach (var item in indexFitness)
		//{
		//	Debug.Log(item.Index + " : " + item.Fitness);
		//}

	}



	// Egyenlőre egyszerre történik a rekombináció és a mutáció
	private void RecombineAndMutate()
	{
		int index;
		float mutation;
		float mutationRateMinimum = (1 - mutationRatePercent);
		float mutationRateMaximum = (1 + mutationRatePercent);


		for (int i = 0; i < CarCount; i++)  // melyik autó
		{
			for (int j = 0; j < carNetworks[i].NeuronLayers.Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < carNetworks[i].NeuronLayers[j].NeuronWeights.Length; k++) // melyik neuronja
				{
					for (int l = 0; l < carNetworks[i].NeuronLayers[j].NeuronWeights[0].Length; l++) // melyik súlya
					{
						if (i == indexFitness[0].Index)
						{
							carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								savedCarNetwork[i, j, k, l];
						}
						else
						{

							mutation = Random.Range(mutationRateMinimum, mutationRateMaximum);
							// 50% eséllyel örököl az egyik szülőtől.
							// carPairs[i] a két szülő indexét tartalmazza
							index = carPairs[i][Random.Range(0, 2)];

							// 50% eséllyel mutálódik
							if (Random.Range(0, 2) == 0)
							{
								carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								savedCarNetwork[index, j, k, l] * mutation;
							}
							else
							{
								carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
									savedCarNetwork[index, j, k, l];
							}
						}
					}
				}
			}
		}
	}



	// Egy új generáció spawnolását végzi
	private void CreateNewGeneration()
	{
		// Ha nincs már működő autó, akkor megtörténhet az új spawn
		if (carsAliveCount == 0)
		{

			// Egy kis várakozási idő, miután minden autó leállt (nem feltétlen kell)
			waitingTime += Time.deltaTime;
			if (waitingTime >= 0.5f)
			{

				CalculateAverageFitness();

				// Elmenti az összes autó neurális hálóját
				SaveNeuralNetworks();

				// Az autók rendezése fitness szerint
				SortCarsByFitness();


				#region Régi párválasztás
				// Kiválasztja az egyik autót
				// és párba állítja az egyik rosszabbik 50%ban lévő autóval.

				//	int fatherIndex = Random.Range(0, (int)(CarCount / 2));
				//	int badCarIndex = indexFitness[Random.Range((int)(CarCount / 2), CarCount)].Index;


				//// Random párokat készít. 
				//// Egy autó nem lehet párja önmagának, KIVÉTEL a legjobb fitness-el rendelkező autó
				//// Mivel itt csak egy helyen lesz önmagával párban a legjobb, így ő teljes egészében továbbjut
				//// a következő generációba, változás nélkül.
				//for (int i = 0; i < CarCount; i++)
				//{

				//	carPairs[i][0] = indexFitness[i / 2].Index;

				//	int rnd = carPairs[i][0];
				//	while (carPairs[i][0] == rnd)
				//	{
				//		rnd = indexFitness[Random.Range(0, (int)(CarCount / 2))].Index;
				//	}
				//	carPairs[i][1] = rnd;

				//}
				//// Egy db a rosszabbik 50%ból származik!
				//carPairs[fatherIndex][1] = badCarIndex;
				#endregion
				

				TournamentSelection();

				#region kiíratás
				//for (int i = 0; i < carPairs.Length; i++)
				//{
				//	string str = carPairs[i][0] + " párja " + carPairs[i][1];
				//	Debug.Log(str);
				//}
				#endregion

				// Megvannak a párok indexei, jöhet a rekombináció
				RecombineAndMutate();

				// Autók respawnolása
				for (int i = 0; i < CarCount; i++)
				{
					Cars[i].LastFitness = 0;
					SpawnFromPool(transform.position, transform.rotation);
				}

				waitingTime = 0;
				carsAliveCount = CarCount;
				freezeTimeLeft = timeOut;
				globalTimeLeft = globalTimeOut;
			}
		}
	}



	// Ellenőrzi az autókat, és lefagyasztja amelyik nem elég gyors.
	private void FreezeSlowCars()
	{
		// Az összes autónak ennyi idő alatt kell eljutnia valameddig, azután FREEZE
		globalTimeLeft -= Time.deltaTime;
		if (globalTimeLeft <= 0)
		{
			for (int i = 0; i < CarCount; i++)
			{
				Cars[i].Transform.gameObject.GetComponent<CarController>().Freeze();
				Cars[i].LastFitness = 0;
			}
			globalTimeLeft = globalTimeOut;
		}

		freezeTimeLeft -= Time.deltaTime;
		if (freezeTimeLeft <= 0)
		{
			for (int i = 0; i < CarCount; i++)
			{

				// Ha nem nőtt 10 másodperc alatt az autó fitness értéke
				// legalább 10-zel, akkor lefagyasztja az autót
				if (Cars[i].Fitness > Cars[i].LastFitness + 10)
				{
					Cars[i].LastFitness = Cars[i].Fitness;
				}
				else
				{
					Cars[i].Transform.gameObject.GetComponent<CarController>().Freeze();
					Cars[i].LastFitness = 0;
				}

			}
			freezeTimeLeft = timeOut;
		}
	}

	private void CalculateAverageFitness()
	{

		double avg = 0;
		for (int i = 0; i < CarCount; i++)
		{
			avg += Cars[i].Fitness;
		}
		avg /= CarCount;

		avgFitness.Add(avg);
		//graphMaker.AddDataPointInOrder(new Vector2(GenerationCount++, (int)avg ));
		//graphMaker.RedrawGraph();
		GenerationCount++;

		double median = 0;

		// Cars[] tömbben az autók
		// indexFitness[] tömbben az autók indexei, fitness szerint sorbarendezve
		// indexfitness[CarCount/2].Index = A sorbarendezett autók közül a középső indexe 
		median = Cars[indexFitness[CarCount / 2].Index].Fitness;
		medianFitness.Add(median);

	}

	public void SaveFitnessData()
	{
		// AVERAGE
		GameLogger.WriteAvgFitnessData("NEW STATS\n\n");
		foreach (double item in avgFitness)
		{
			GameLogger.WriteAvgFitnessData(item.ToString());
		}
		GameLogger.WriteAvgFitnessData("\n\n");


		// MEDIAN
		GameLogger.WriteMedianFitnessData("NEW STATS\n\n");
		foreach (double item in medianFitness)
		{
			GameLogger.WriteMedianFitnessData(item.ToString());
		}
		GameLogger.WriteMedianFitnessData("\n\n");

	}

	private void TournamentSelection() {
		TournamentSelectionBase(0);
		TournamentSelectionBase(1);
	}

	private void TournamentSelectionBase(int parentIndex)
	{
		int selectionPressure = 3;

		// Holds the choosen cars indexes for the round
		List<int> picked = new List<int>();

		int paired = 0;

		#region Full random az egyik szülő
		// Az első szülő full random
		//for (int i = 0; i < CarCount; i++)
		//{
		//	carPairs[i][0] = Random.Range(0, CarCount);
		//}
		#endregion


		// amíg meg nincs meg az összes pár, új tournament
		while (paired < CarCount)
		{
			#region Jelenlegi tournament inicializálása
			List<int> tournament = new List<int>();
			for (int i = 0; i < CarCount; i++)
			{
				tournament.Add(i);
			}
			#endregion

			// amíg van elég versenyző a tournamenten belül (és még kell pár), versenyzők kiválasztása
			while (tournament.Count >= selectionPressure && paired < CarCount)
			{
				// a kiválasztottak kiürítése (ha nem üres)
				picked.Clear();

				// amíg meg nincs mindegyik versenyző
				while (picked.Count != selectionPressure)
				{
					int current = tournament[Random.Range(0, tournament.Count)];
					if (!picked.Contains(current))
					{
						picked.Add(current);
						tournament.Remove(current);
					}
				}
				// párosítás
				carPairs[paired][parentIndex] = GetTournamentBestIndex(picked);
				paired++;
			}
		}

		// Elvileg a Recombine-ban benne van, hogy a legjobb autó maradjon...
		//carPairs[carPairs.Length][0] = GetBestCarIndex();
		//carPairs[carPairs.Length][1] = GetBestCarIndex();

	}

	private int GetTournamentBestIndex(List<int> picked)
	{
		int bestIndex = int.MinValue;
		double highestFitness = double.MinValue;
		foreach (var thispicked in picked)
		{
			double currentFitness = 0;
			foreach (var infi in indexFitness)
			{
				if (infi.Index == thispicked)
				{
					currentFitness = infi.Fitness;
				}
			}
			if (currentFitness > highestFitness)
			{
				highestFitness = currentFitness;
				bestIndex = thispicked;
			}
		}
		return bestIndex;
	}

}
