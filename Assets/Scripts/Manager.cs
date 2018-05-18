using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
	public static Manager Instance = null;

	#region Options
	public int CarCount { get; set; }
	public int SelectionMethod { get; set; }
	public int MutationChance { get; set; }
	public float MutationRate { get; set; }
	public int LayersCount { get; set; }
	public int NeuronPerLayerCount { get; set; }
	public int TrackNumber { get; set; }
	public int CarSensorCount { get; set; }
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
	private GeneticAlgorithm GA;
	private Queue<GameObject> carPool;
	private bool firstStart = true;
	private const float freezeTimeOut = 10.0f;
	private const float globalTimeOut = 40.0f;

	[HideInInspector] public float freezeTimeLeft = freezeTimeOut;
	[HideInInspector] public float globalTimeLeft = globalTimeOut;

	[SerializeField] private GameObject[] TrackPrefabs;
	[HideInInspector] public GameObject CurrentTrack;
	[SerializeField] private CameraDrone cameraDrone;

	public Car[] Cars;
	private GameObject playerCar;
	public double PlayerFitness { get; set; }
	private bool isPlayerAlive = false;

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

	// Meghívódik minden képfrissítéskor
	void FixedUpdate()
	{
		ManageTime();
	}


	/// <summary>
	/// Létrehozza az autókat a poolba. Ezek még nem aktív autók!
	/// Aktiválni EGY autót a SpawnFromPool metódussal lehet.
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
		}
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

	// A player irányította autó spawnolása a kezdőpozícióba.
	// Az autók pozíció adatait stb. visszaállítja defaultra.
	// TODO!!
	public GameObject SpawnPlayerCar(Vector3 position, Quaternion rotation)
	{
		playerCar.GetComponent<CarController>().IsAlive = true;
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

	// A START gomb hívja meg ezt a metódust
	public void InitGame()
	{
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

		Debug.Log("CarCount = " + CarCount);

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
		DontDestroyOnLoad(CurrentTrack);
		CurrentTrack.SetActive(true);
	}

	void InitCars()
	{
		// TODO: Inputs tömb mérete nagyobb, ha az autó inputként
		// megkapja a sarkokat is!!

		CarSensorCount = 5;

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
	/// Az időket kezeli, meghívja az ellenőrző-fagyasztó metódusokat
	/// amikor letelik az időkorlát.
	/// </summary>
	void ManageTime()
	{
		globalTimeLeft -= Time.deltaTime;
		freezeTimeLeft -= Time.deltaTime;

		if (globalTimeLeft <= 0)
		{
			// TODO: FREEZE all of my bruddah
			globalTimeLeft = globalTimeOut;
		}

		if (freezeTimeLeft <= 0)
		{
			// TODO: Ellenőrzi a különbséget 
			// a lastfitness és a fitness között minden autónál
			freezeTimeLeft = freezeTimeOut;
		}

	}

}
