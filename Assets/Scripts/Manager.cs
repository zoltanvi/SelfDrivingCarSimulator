using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
	public static Manager Instance = null;

	#region Developer options
	public bool LogNetworkData { get; set; }
	#endregion

	#region Options
	public int CarCount { get; set; }
	public int SelectionMethod { get; set; }
	public int MutationChance { get; set; }
	public float MutationRate { get; set; }
	public int LayersCount { get; set; }
	public int NeuronPerLayerCount { get; set; }
	public int TrackNumber { get; set; }
	[Range(1, 30)]
	public int CarSensorCount = 5;
	[Range(10, 40)]
	public int CarSensorLength = 25;

	public double Bias { get; set; }
	#endregion

	#region Prefabs & materials
	[SerializeField] protected GameObject blueCarPrefab;
	[SerializeField] protected GameObject redCarPrefab;
	[SerializeField] protected GameObject blueCarMesh;

	[SerializeField] protected Material blueCarMat;
	[SerializeField] protected Material blueCarMatTrans;
	[SerializeField] protected Material wheelMat;
	[SerializeField] protected Material wheelMatTrans;
	#endregion

	public bool GotOptionValues = false;
	public bool ManualControl = false;

	private GameObject GAGameObject;
	private GeneticAlgorithm GA = null;
	private Queue<GameObject> carPool;
	private bool firstStart = true;
	[SerializeField] private GameObject UIStats;
	[SerializeField] private GameObject inGameMenu;

	[HideInInspector] public UIPrinter myUIPrinter;

	public int bestCarID = 0;
	private const float freezeTimeOut = 10.0f;
	private const float globalTimeOut = 40.0f;

	public float freezeTimeLeft = freezeTimeOut;
	public float globalTimeLeft = globalTimeOut;

	[SerializeField] private GameObject[] TrackPrefabs;
	[SerializeField] private GameObject[] WayPointPrefabs;

	[HideInInspector] public GameObject CurrentTrack;
	[HideInInspector] public GameObject CurrentWaypoint;

	[SerializeField] private CameraDrone cameraDrone;

	public Car[] Cars;

	#region Player változói
	private GameObject playerCar;
	private CarController playerCarController;
	public double PlayerFitness { get; set; }
	[HideInInspector] public bool isPlayerAlive = false;
	#endregion

	[HideInInspector] public List<double> maxFitness = new List<double>();
	[HideInInspector] public List<double> medianFitness = new List<double>();

	[HideInInspector] public int AliveCount { get; set; }

	/// <summary>
	/// Példányosít egyet önmagából (Singleton)
	/// </summary>
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		DontDestroyOnLoad(UIStats);
		DontDestroyOnLoad(inGameMenu);
		myUIPrinter = UIStats.GetComponent<UIPrinter>();
		UIStats.SetActive(false);
		inGameMenu.SetActive(false);
	}

	void Update()
	{
		ControlMenu();
	}

	// Meghívódik minden képfrissítéskor
	void FixedUpdate()
	{

		// Csak ha elindult a szimuláció, akkor fussanak le a következők
		if (!firstStart)
		{
			ManageTime();

			if (!ManualControl)
			{
				bestCarID = GetBestID();
				myUIPrinter.FitnessValue = Cars[bestCarID].Fitness;
				cameraDrone.CameraTarget = Cars[bestCarID].Transform;

				myUIPrinter.ConsoleMessage = "";
				for (int i = 0; i < Cars[bestCarID].Inputs.Length; i++)
				{
					myUIPrinter.ConsoleMessage += string.Format("> {0:0.000}\n", Cars[bestCarID].Inputs[i]);
				}
			}
			else
			{
				myUIPrinter.FitnessValue = PlayerFitness;
			}


		}


	}


	/// <summary>
	/// Létrehozza az autókat a poolba. Ezek még nem aktív autók!
	/// 
	/// Aktiválni EGY autót a SpawnFromPool metódussal lehet.
	/// 
	/// Aktiválni AZ ÖSSZES autót a SpawnCars metódussal lehet.
	/// </summary>
	void InstantiateCars()
	{
		carPool = new Queue<GameObject>();

		for (int i = 0; i < CarCount; i++)
		{
			GameObject obj = Instantiate(blueCarPrefab, transform.position, transform.rotation);

			obj.SetActive(false);
			carPool.Enqueue(obj);

			Cars[i].GameObject = obj;
			Cars[i].CarController = obj.GetComponent<CarController>();
			Cars[i].NeuralNetwork = obj.GetComponent<NeuralNetwork>();
			Cars[i].CarController.ID = i;
			Cars[i].Transform = obj.transform;
			Cars[i].Transform.name = "Car_" + i;
			Cars[i].IsAlive = true;
		}
	}

	/// <summary>
	/// Létrehozza a player autóját. Ez még nem aktív autó!
	/// Aktiválni a player autóját a SpawnPlayerCar metódussal lehet.
	/// </summary>
	void InstantiatePlayerCar()
	{
		playerCar = Instantiate(redCarPrefab, transform.position, transform.rotation);

		playerCarController = playerCar.GetComponent<CarController>();
		playerCarController.IsPlayerControlled = true;
		playerCarController.ID = -1;
		playerCar.transform.name = "PlayerCar(RED)";
		playerCar.SetActive(false);
	}

	/// <summary>
	/// Lespawnolja az összes autót a spawnpointra.
	/// </summary>
	void SpawnCars()
	{
		// Az autók spawnolása
		for (int i = 0; i < CarCount; i++)
		{
			SpawnFromPool(transform.position, transform.rotation);
		}
	}


	/// <summary>
	/// Az inicializált autókból a sorra következőt lespawnolja a kezdőpozícióba.
	/// Az autók pozíció adatait, stb. visszaállítja defaultra.
	/// </summary>
	/// <param name="position">A hely ahova spawnoljon</param>
	/// <param name="rotation">A szög amelyre spawnoljon.</param>
	/// <returns>Visszatér a spawnolt autó GameObjectjével.</returns>
	public GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
	{
		GameObject objectToSpawn = carPool.Dequeue();
		objectToSpawn.GetComponent<CarController>().IsAlive = true;

		Rigidbody objectRigidbody = objectToSpawn.GetComponent<Rigidbody>();
		objectRigidbody.isKinematic = false;
		objectRigidbody.velocity = new Vector3(0, 0, 0);
		objectRigidbody.angularVelocity = new Vector3(0, 0, 0);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		// Ha már volt első spawnolás, akkor az autó fitness értékeinek visszaállítása.
		// (Errort dobna ha első spawnoláskor elérné ezt a kódot!)
		if (!firstStart)
		{
			objectToSpawn.GetComponent<FitnessMeter>().Reset();
		}

		carPool.Enqueue(objectToSpawn);
		return objectToSpawn;
	}

	/// <summary>
	/// A player autóját lespawnolja a kezdőpozícióba. 
	/// Az autó pozíció adatait stb. visszaállítja defaultra.
	/// </summary>
	/// <param name="position">A hely ahova spawnoljon.</param>
	/// <param name="rotation">A szög amelyre spawnoljon.</param>
	/// <returns>Visszatér a spawnolt autó GameObjectjével.</returns>
	public GameObject SpawnPlayerCar(Vector3 position, Quaternion rotation)
	{
		playerCarController.IsAlive = true;
		Rigidbody objectRigidbody = playerCar.GetComponent<Rigidbody>();
		objectRigidbody.isKinematic = false;
		objectRigidbody.velocity = new Vector3(0, 0, 0);
		objectRigidbody.angularVelocity = new Vector3(0, 0, 0);
		playerCar.transform.position = position;
		playerCar.transform.rotation = rotation;
		playerCar.SetActive(true);
		isPlayerAlive = true;
		// Ha már volt első spawnolás, akkor az autó fitness értékeinek visszaállítása.
		// (Errort dobna ha első spawnoláskor elérné ezt a kódot!)
		if (!firstStart)
		{
			playerCar.GetComponent<FitnessMeter>().Reset();
		}
		return playerCar;
	}

	// TODO!!
	public void JoinGame()
	{
		ManualControl = !ManualControl;
		CheckCarMaterials();

	}

	/// <summary>
	/// Ellenőrzi, hogy becsatlakozott-e a játékos, és aszerint váltja
	/// az autók materialját átlátszóra.
	/// </summary>
	private void CheckCarMaterials()
	{

		// Kicseréli az autó materialokat attól függően, hogy játszik-e a player
		if (!ManualControl)
		{
			Transform blueCarWheels = blueCarMesh.transform.GetChild(1);
			blueCarMesh.transform.GetChild(0).GetComponent<MeshRenderer>().material = blueCarMat;
			blueCarWheels.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = wheelMat;
			blueCarWheels.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material = wheelMat;
			blueCarWheels.GetChild(2).GetChild(0).GetComponent<MeshRenderer>().material = wheelMat;
			blueCarWheels.GetChild(3).GetChild(0).GetComponent<MeshRenderer>().material = wheelMat;
		}
		else
		{
			Transform blueCarWheels = blueCarMesh.transform.GetChild(1);
			blueCarMesh.transform.GetChild(0).GetComponent<MeshRenderer>().material = blueCarMatTrans;
			blueCarWheels.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = wheelMatTrans;
			blueCarWheels.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material = wheelMatTrans;
			blueCarWheels.GetChild(2).GetChild(0).GetComponent<MeshRenderer>().material = wheelMatTrans;
			blueCarWheels.GetChild(3).GetChild(0).GetComponent<MeshRenderer>().material = wheelMatTrans;
		}

	}

	/// <summary>
	/// Meghívódik, amikor betöltött a következő scene.
	/// Ez indítja a játékot!
	/// </summary>
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		InitGame();
		StartGame();
	}

	public void InitGame()
	{
		Bias = 1.0;
		InitTrack();
		InitGenetic();
		InitCars();

		AliveCount = CarCount;
		cameraDrone.enabled = false;

	}

	public void StartGame()
	{
		CheckCarMaterials();
		InstantiateCars();
		SpawnCars();

		UIStats.SetActive(true);

		cameraDrone.CameraTarget = Cars[0].Transform;
		cameraDrone.enabled = true;
		// Első spawnolás megtörtént
		firstStart = false;
	}

	/// <summary>
	/// A genetikus algoritmust inicializálja, attól függően, hogy
	/// mi volt az options menüben kiválasztva.
	/// </summary>
	void InitGenetic()
	{
		// Példányosít egy GeneticAlgorithm objektumot
		GAGameObject = new GameObject();
		GAGameObject.transform.name = "GeneticAlgorithm";

		switch (SelectionMethod)
		{
			// Tournament selection
			case 0:
				GAGameObject.AddComponent<GeneticAlgorithmTournament>();
				GA = GAGameObject.GetComponent<GeneticAlgorithmTournament>();
				break;
			// Top 50% selection
			case 1:
				GAGameObject.AddComponent<GeneticAlgorithmTopHalf>();
				GA = GAGameObject.GetComponent<GeneticAlgorithmTopHalf>();
				break;
			// Tournament + worst 20% full random
			case 2:
				GAGameObject.AddComponent<GeneticAlgorithmWorstRandom>();
				GA = GAGameObject.GetComponent<GeneticAlgorithmWorstRandom>();
				break;
			// Tournament selection
			default:
				GAGameObject.AddComponent<GeneticAlgorithmTournament>();
				GA = GAGameObject.GetComponent<GeneticAlgorithmTournament>();
				break;
		}

	}


	void InitTrack()
	{
		// TODO: pálya választó
		TrackNumber = 0;

		CurrentTrack = Instantiate(TrackPrefabs[TrackNumber], transform.position, transform.rotation);
		CurrentWaypoint = Instantiate(WayPointPrefabs[TrackNumber], transform.position, transform.rotation);

		DontDestroyOnLoad(CurrentTrack);
		DontDestroyOnLoad(CurrentWaypoint);
		CurrentTrack.SetActive(true);
		CurrentWaypoint.SetActive(true);
	}

	void InitCars()
	{
		// TODO: Inputs tömb mérete nagyobb, ha az autó inputként
		// megkapja a sarkokat is!!

		Cars = new Car[CarCount];
		for (int i = 0; i < CarCount; i++)
		{
			Cars[i] = new Car
			{
				ID = i,
				Fitness = 0,
				Inputs = new double[CarSensorCount + 1],
				PrevFitness = 0
			};
		}
	}

	void LoadGame()
	{
		// TODO
	}

	void SaveGame()
	{
		// TODO
	}

	/// <summary>
	/// Az időket kezeli, meghívja az ellenőrző/fagyasztó metódusokat
	/// amikor letelnek az időkorlátok.
	/// </summary>
	void ManageTime()
	{
		globalTimeLeft -= Time.deltaTime;
		freezeTimeLeft -= Time.deltaTime;

		// Ha letelik a globális idő
		if (globalTimeLeft <= 0)
		{
			// Lefagyasztja az összes autót
			for (int i = 0; i < CarCount; i++)
			{
				Cars[i].CarController.Freeze();
				Cars[i].PrevFitness = 0;
			}
			// Ha játszik a player, az ő autóját is megfagyasztja
			if (ManualControl)
			{
				playerCar.transform.gameObject.GetComponent<CarController>().Freeze();
			}

			// Az időt visszaállítja, így kezdődhet elölről.
			globalTimeLeft = globalTimeOut;
		}

		if (freezeTimeLeft <= 0)
		{

			for (int i = 0; i < CarCount; i++)
			{
				// Ha nem nőtt 10 másodperc alatt az autó fitness értéke
				// legalább 10-zel, akkor lefagyasztja az autót
				if (Cars[i].Fitness > Cars[i].PrevFitness + 10)
				{
					Cars[i].PrevFitness = Cars[i].Fitness;
				}
				else
				{
					Cars[i].CarController.Freeze();
					Cars[i].PrevFitness = 0;
				}
			}

			// Az időt visszaállítja, így kezdődhet elölről.
			freezeTimeLeft = freezeTimeOut;
		}

	}

	/// <summary>
	/// Lefagyasztja a paraméterben kapott autót, majd logolja
	/// az autóhoz tartozó neurális háló súlyait + az autó fitnessét.
	/// </summary>
	/// <param name="carRigidbody">A megfagyasztandó autó rigidbodyja</param>
	/// <param name="carID">A megfagyasztandó autó ID-je</param>
	/// <param name="isAlive">Az autó életben van-e</param>
	public void FreezeCar(Rigidbody carRigidbody, int carID, bool isAlive)
	{
		if (isAlive)
		{
			carRigidbody.isKinematic = true;

			if (carID != -1)
			{
				AliveCount--;

				if (LogNetworkData)
				{
					NeuralNetwork tmp2 = Cars[carID].Transform.gameObject.GetComponent<NeuralNetwork>();

					#region Súlyok és fitness értékek fájlba írása
					string carNNWeights = carID + ". car:\n";
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
					string dataText = "Maximum fitness: " + Cars[carID].Fitness + "\n\n";
					GameLogger.WriteData(dataText);
					#endregion
				}

			}
			else
			{
				isPlayerAlive = false;
			}

		}
	}

	/// <summary>
	/// Visszaállítja az időket + mennyi autó van életben
	/// </summary>
	public void SetBackTimes()
	{
		AliveCount = CarCount;
		freezeTimeLeft = freezeTimeOut;
		globalTimeLeft = globalTimeOut;
	}


	/// <summary>
	/// Visszaadja a legmagasabb fitness értékkel rendelkező
	/// életben lévő autó ID-jét.
	/// </summary>
	/// <returns>A legjobb autó ID-jét adja vissza.</returns>
	private int GetBestID()
	{
		int bestID = 0;
		double maxFitness = double.MinValue;

		for (int i = 0; i < CarCount; i++)
		{
			if (Cars[i].IsAlive)
			{
				if (maxFitness < Cars[i].Fitness)
				{
					maxFitness = Cars[i].Fitness;
					bestID = Cars[i].ID;
				}
			}
		}

		return bestID;
	}

	private void ControlMenu()
	{
		// Ha az ESC billentyűt lenyomták
		if (Input.GetButtonDown("Cancel"))
		{
			// Ha már nem a főmenüben áll
			if (GotOptionValues)
			{
				// Ha aktív volt, eltünteti, ha nem volt aktív, előhozza
				if (inGameMenu.activeSelf)
				{
					inGameMenu.SetActive(false);
				}
				else
				{
					inGameMenu.SetActive(true);
				}
			}
		}

	}


}
