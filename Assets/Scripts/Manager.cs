using Crosstales.FB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.PostProcessing;


public class Manager : MonoBehaviour
{

	public struct Car
	{
		public int Id;
		public float Fitness;
		public float[] Inputs;
		public GameObject GameObject;
		public Transform Transform;
		public CarController CarController;
		public NeuralNetwork NeuralNetwork;
		public float PrevFitness;
		public bool IsAlive;
	}

	#region VISIBLE IN INSPECTOR
	[Header("Car settings")]
	[Range(1, 30)] public int CarSensorCount = 5;
	[Range(10, 40)] public int CarSensorLength = 25;

	[Header("Save object - this contains the saved data")]
	// Ebben az objektumban van tárolva az elmentett / betöltött adatok.
	public GameSave Save;
	#endregion


	#region PUBLIC BUT NOT VISIBLE IN INSPECTOR
	public Car[] Cars;
	public int CarCount { get; set; }
	public int SelectionMethod { get; set; }
	public int MutationChance { get; set; }
	public float MutationRate { get; set; }
	public int LayersCount { get; set; }
	public int NeuronPerLayerCount { get; set; }
	public int TrackNumber { get; set; }
	public float Bias { get; set; }
	public bool Navigator { get; set; }
	public bool DemoMode { get; set; }
	[HideInInspector] public bool GotOptionValues;
	[HideInInspector] public bool InGame;
	[HideInInspector] public bool ManualControl;
	[HideInInspector] public bool WasItALoad;
	[HideInInspector] public UIPrinter MyUIPrinter;
	[HideInInspector] public List<float> MaxFitness = new List<float>();
	[HideInInspector] public List<float> MedianFitness = new List<float>();
	[HideInInspector] public int AliveCount { get; set; }
	[HideInInspector] public int BestCarId;
	[HideInInspector] public float FreezeTimeLeft = freezeTimeOut;
	[HideInInspector] public float GlobalTimeLeft = globalTimeOut;
	[HideInInspector] public GameObject CurrentTrack;
	[HideInInspector] public GameObject CurrentWayPoint;
	[HideInInspector] public float PlayerFitness { get; set; }
	[HideInInspector] public bool IsPlayerAlive = false;
	[HideInInspector] public GameObject RayHolderRoot;
	[HideInInspector] public GameObject CarHolderRoot;
	[HideInInspector] public GameObject PlayerCar;
	#endregion


	#region PRIVATE
	private GameObject GAGameObject;
	private GeneticAlgorithm GA;
	private Queue<GameObject> carPool;
	private bool firstStart = true;
	private bool playerFirstStart = true;
	private CarController playerCarController;
	private const float freezeTimeOut = 10.0f;
	private const float globalTimeOut = 40.0f;
	private Shader standardShader;
	private Shader transparentShader;
	private Color visibleColor;
	private Color transparentColor;
	private string[] cheatCode;
	private int cheatIndex;
	private string popUpText;
	private Master master;
	#endregion


	private void Start()
	{
		master = Master.Instance;
		if(RandomHelper.DefaultSeed == RandomHelper.Seed)
		{
			RandomHelper.GenerateNewSeed();
		}

		master.MenuController.SetSeedText(RandomHelper.Seed.ToString());
		Debug.Log("Seed beállítva!");

		MyUIPrinter = master.UIStats.GetComponent<UIPrinter>();

		Save = new GameSave();
		transparentShader = Shader.Find("Standard (Specular setup)");
		standardShader = Shader.Find("Standard");
		visibleColor = new Color(1, 1, 1, 1.0f);
		transparentColor = new Color(1, 1, 1, 0.0f);
		cheatCode = new string[] { "n", "e", "r", "f" };
		cheatIndex = 0;
	}

	private void Update()
	{
		ControlMenu();
	}

	// Meghívódik minden képfrissítéskor
	private void FixedUpdate()
	{

		// Csak ha elindult a szimuláció, akkor fussanak le a következők
		if (firstStart) return;

		ManageTime();

		// Ha játszik a játékos és még életben van, őt követi a kamera..
		// Különben a legjobb élő autót
		if (ManualControl && IsPlayerAlive)
		{
			master.cameraController.CameraTarget = PlayerCar.transform;
			MyUIPrinter.FitnessValue = PlayerFitness;
			MyUIPrinter.ConsoleMessage = string.Empty;
		}
		else
		{
			BestCarId = GetBestId();
			MyUIPrinter.FitnessValue = Cars[BestCarId].Fitness;
			master.cameraController.CameraTarget = Cars[BestCarId].Transform;

			StringBuilder sb = new StringBuilder();
			int carInputsLength = Cars[BestCarId].Inputs.Length;
			for (int i = 0; i < carInputsLength; i++)
			{
				sb.Append(string.Format("> {0:0.000}\n", Cars[BestCarId].Inputs[i]));
			}
			MyUIPrinter.ConsoleMessage = sb.ToString();
		}

	}


	/// <summary>
	/// Létrehozza az autókat a poolba. Ezek még nem aktív autók!
	/// Aktiválni EGY autót a SpawnFromPool metódussal lehet.
	/// Aktiválni AZ ÖSSZES autót a SpawnCars metódussal lehet.
	/// </summary>
	private void InstantiateCars()
	{
		carPool = new Queue<GameObject>();
		RayHolderRoot = new GameObject("RayHolderDeletable");
		CarHolderRoot = new GameObject("CarHolderDeletable");
		for (int i = 0; i < CarCount; i++)
		{
			GameObject obj = Instantiate(master.BlueCarPrefab, transform.position, transform.rotation);

			obj.SetActive(false);
			carPool.Enqueue(obj);

			Cars[i].GameObject = obj;
			Cars[i].CarController = obj.GetComponent<CarController>();
			Cars[i].NeuralNetwork = obj.GetComponent<NeuralNetwork>();
			Cars[i].CarController.Id = i;
			Cars[i].Transform = obj.transform;
			Cars[i].Transform.parent = CarHolderRoot.transform;
			Cars[i].Transform.name = "Car_" + i;
			Cars[i].IsAlive = true;
		}


	}

	/// <summary>
	/// Létrehozza a player autóját. Ez még nem aktív autó!
	/// Aktiválni a player autóját a SpawnPlayerCar metódussal lehet.
	/// </summary>
	private void InstantiatePlayerCar()
	{
		PlayerCar = Instantiate(master.RedCarPrefab, transform.position, transform.rotation);

		playerCarController = PlayerCar.GetComponent<CarController>();
		playerCarController.IsPlayerControlled = true;
		playerCarController.Id = -1;
		PlayerCar.transform.name = "PlayersCarDeletable";
		PlayerCar.SetActive(false);
	}

	/// <summary>
	/// Lespawnolja az összes autót a spawnpointra.
	/// </summary>
	private void SpawnCars()
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
		Rigidbody objectRigidbody = PlayerCar.GetComponent<Rigidbody>();
		objectRigidbody.isKinematic = false;
		objectRigidbody.velocity = new Vector3(0, 0, 0);
		objectRigidbody.angularVelocity = new Vector3(0, 0, 0);
		PlayerCar.transform.position = position;
		PlayerCar.transform.rotation = rotation;
		PlayerCar.SetActive(true);
		IsPlayerAlive = true;
		// Ha már volt első spawnolás, akkor az autó fitness értékeinek visszaállítása.
		// (Errort dobna ha első spawnoláskor elérné ezt a kódot!)
		if (!playerFirstStart)
		{
			PlayerCar.GetComponent<FitnessMeter>().Reset();
		}
		return PlayerCar;
	}


	public void JoinGame()
	{
		ManualControl = true;
		CheckCarMaterials();
		SpawnPlayerCar(transform.position, transform.rotation);
		playerFirstStart = false;
	}

	public void DisconnectGame()
	{
		ManualControl = false;
		CheckCarMaterials();
		IsPlayerAlive = false;
		PlayerCar.SetActive(false);
		playerFirstStart = true;

	}

	/// <summary>
	/// Ellenőrzi, hogy becsatlakozott-e a játékos, és aszerint váltja
	/// az autók materialját átlátszóra.
	/// </summary>
	private void CheckCarMaterials()
	{
		// Átlátszóra állítja az autókat, ha a játékos becsatlakozott, 
		// visszaállítja, ha már nem játszik.

		Renderer[] rend;

		if (!ManualControl)
		{
			for (int i = 0; i < CarCount; i++)
			{
				rend = Cars[i].Transform.GetComponentsInChildren<Renderer>();

				foreach (Renderer carRenderer in rend)
				{
					carRenderer.material.shader = standardShader;
					carRenderer.material.SetColor("_Color", visibleColor);
					carRenderer.material.SetFloat("_Mode", 0);

					carRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					carRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					carRenderer.material.SetInt("_ZWrite", 1);
					carRenderer.material.DisableKeyword("_ALPHATEST_ON");
					carRenderer.material.DisableKeyword("_ALPHABLEND_ON");
					carRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					carRenderer.material.renderQueue = -1;
				}

			}
		}
		else
		{

			for (int i = 0; i < CarCount; i++)
			{
				rend = Cars[i].Transform.GetComponentsInChildren<Renderer>();

				foreach (Renderer carRenderer in rend)
				{
					carRenderer.material.shader = transparentShader;
					carRenderer.material.SetColor("_Color", transparentColor);
					carRenderer.material.SetFloat("_Mode", 3);

					carRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					carRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					carRenderer.material.SetInt("_ZWrite", 0);
					carRenderer.material.DisableKeyword("_ALPHATEST_ON");
					carRenderer.material.DisableKeyword("_ALPHABLEND_ON");
					carRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
					carRenderer.material.renderQueue = 3000;
				}

			}
		}

	}

	public void StartGame()
	{
		// Ha demó módban van, akkor betölti a resource mappából a demó mentést
		if (DemoMode)
		{
			TextAsset asset = Resources.Load("demoSave") as TextAsset;
			if (asset == null)
			{
				ShowPopUp(TextResources.GetValue("popup_demo_save_missing"));
				throw new NullReferenceException(TextResources.GetValue("popup_demo_save_missing"));
			}

			MemoryStream stream = new MemoryStream(asset.bytes);
			BinaryFormatter bf = new BinaryFormatter();
			Save = (GameSave)bf.Deserialize(stream);


			SelectionMethod = Save.SelectionMethod;
			MutationChance = Save.MutationChance;
			MutationRate = Save.MutationRate;
			CarCount = Save.CarCount;
			LayersCount = Save.LayersCount;
			NeuronPerLayerCount = Save.NeuronPerLayerCount;
			Navigator = Save.Navigator;
			TrackNumber = Save.TrackNumber;
			WasItALoad = true;
		}

		InitGame();
		PlayGame();
	}

	public void InitGame()
	{
		Bias = 1.0f;
		InitTrack();
		InitCars();
		InitGenetic();


		AliveCount = CarCount;
		master.cameraController.enabled = false;
		master.Camera.GetComponent<PostProcessingBehaviour>().enabled = true;

	}

	public void PlayGame()
	{
		InstantiateCars();
		// A player autóját is példányosítja a játék indulásakor
		InstantiatePlayerCar();
		SpawnCars();

		master.UIStats.SetActive(true);

		master.cameraController.CameraTarget = Cars[0].Transform;
		master.cameraController.enabled = true;
		// Első spawnolás megtörtént
		firstStart = false;
		InGame = true;

	}

	/// <summary>
	/// A genetikus algoritmust inicializálja, attól függően, hogy
	/// mi volt az options menüben kiválasztva.
	/// </summary>
	private void InitGenetic()
	{
		// Példányosít egy GeneticAlgorithm objektumot
		GAGameObject = new GameObject();
		GAGameObject.transform.name = "GeneticAlgorithmDeletable";

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
			// Roulette wheel selection
			case 3:
				GAGameObject.AddComponent<GeneticAlgorithmRouletteWheel>();
				GA = GAGameObject.GetComponent<GeneticAlgorithmRouletteWheel>();
				break;
			// Tournament selection
			default:
				GAGameObject.AddComponent<GeneticAlgorithmTournament>();
				GA = GAGameObject.GetComponent<GeneticAlgorithmTournament>();
				break;
		}

	}

	private void InitTrack()
	{
		switch (TrackNumber)
		{
			case 0:
				master.minimapCamera.transform.position = new Vector3(-34.0f, 7.0f, 22.8f);
				break;

			case 1:
				master.minimapCamera.transform.position = new Vector3(-1.0f, 7.0f, -2.5f);
				break;

			case 2:
				master.minimapCamera.transform.position = new Vector3(10.0f, 7.0f, 5.0f);
				break;

			default:
				master.minimapCamera.transform.position = new Vector3(-34.0f, 7.0f, 22.8f);
				break;
		}


		CurrentTrack = Instantiate(master.TrackPrefabs[TrackNumber], transform.position, transform.rotation);
		CurrentWayPoint = Instantiate(master.WayPointPrefabs[TrackNumber], transform.position, transform.rotation);
		CurrentTrack.transform.name = "TrackDeletable";
		CurrentWayPoint.transform.name = "WaypointDeletable";
		DontDestroyOnLoad(CurrentTrack);
		DontDestroyOnLoad(CurrentWayPoint);
		CurrentTrack.SetActive(true);
		CurrentWayPoint.SetActive(true);
	}

	private void InitCars()
	{
		// Inputs tömb mérete nagyobb, ha az autó inputként megkapja a következő 3 szöget is!!
		int inputCount;

		if (Navigator)
		{
			inputCount = CarSensorCount + 4;
		}
		else
		{
			inputCount = CarSensorCount + 1;
		}

		Cars = new Car[CarCount];
		for (int i = 0; i < CarCount; i++)
		{
			Cars[i] = new Car
			{
				Id = i,
				Fitness = 0,
				Inputs = new float[inputCount],
				PrevFitness = 0
			};
		}
	}

	public void LoadGame()
	{
		const string extensions = "SAVE";
		string path = FileBrowser.OpenSingleFile(TextResources.GetValue("filebrowser_select_to_load"), string.Empty, extensions);
		Debug.Log(TextResources.GetValue("popup_selected_file") + path);
		ShowPopUp(TextResources.GetValue("popup_selected_file") + path);

		if (string.IsNullOrEmpty(path))
		{
			Debug.LogError(TextResources.GetValue("popup_no_file_selected"));
			ShowPopUp(TextResources.GetValue("popup_no_file_selected"));
			return;
		}

		if (!File.Exists(path) || !Path.GetExtension(path).Equals(".SAVE"))
		{
			ShowPopUp(TextResources.GetValue("popup_wrong_file"));
			Debug.LogError(TextResources.GetValue("popup_wrong_file"));
			return;
		}

		FileStream file;

		using (file = File.Open(path, FileMode.Open))
		{
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				Save = (GameSave)bf.Deserialize(file);
			}
			catch (Exception)
			{
				ShowPopUp(TextResources.GetValue("popup_not_compatible_save"));
				Debug.LogError(TextResources.GetValue("popup_not_compatible_save"));
				return;
			}
		}

		SelectionMethod = Save.SelectionMethod;
		MutationChance = Save.MutationChance;
		MutationRate = Save.MutationRate;
		CarCount = Save.CarCount;
		LayersCount = Save.LayersCount;
		NeuronPerLayerCount = Save.NeuronPerLayerCount;
		Navigator = Save.Navigator;
		MedianFitness = Save.MedianFitness;
		MaxFitness = Save.MaxFitness;

		GotOptionValues = true;
		WasItALoad = true;

		master.StartNewGame();
		master.mainMenuCanvas.SetActive(false);
		master.bgLights.SetActive(false);
	}

    public void SaveGame()
    {
        const string extensions = "SAVE";
        DateTime dateTime = DateTime.Now;
        string timeStamp = dateTime.ToString("yyyyMMdd-HHmmss");
        string path = FileBrowser.SaveFile(TextResources.GetValue("filebrowser_select_save_location"), string.Empty, "CGSave_" + timeStamp, extensions);

        if (path.Length == 0)
        {
            ShowPopUp(TextResources.GetValue("popup_no_folder_selected"));
            Debug.Log(TextResources.GetValue("popup_no_folder_selected"));
            return;
        }
        else
        {
            ShowPopUp(TextResources.GetValue("popup_save_file") + path);
            Debug.Log(TextResources.GetValue("popup_save_file") + path);
        }

        // Először kiírja a jelenlegi neurálnet adatokat egy tömbbe
        GA.SaveNeuralNetworks();

        Save.SelectionMethod = SelectionMethod;
        Save.MutationChance = MutationChance;
        Save.MutationRate = MutationRate;
        Save.CarCount = CarCount;
        Save.LayersCount = LayersCount;
        Save.NeuronPerLayerCount = NeuronPerLayerCount;
        Save.Navigator = Navigator;
        Save.TrackNumber = TrackNumber;
        Save.SavedCarNetworks = GA.SavedCarNetworks;

        if (master.CreateDemoSave)
        {
            Save.GenerationCount = 0;
            Save.MaxFitness = new List<float>();
            Save.MedianFitness = new List<float>();
        }
        else
        {
            Save.GenerationCount = GA.GenerationCount;
            Save.MaxFitness = MaxFitness;
            Save.MedianFitness = MedianFitness;
        }


        FileStream file;
        using (file = File.Create(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, Save);
        }

    }


	public void SaveStats()
	{
		const string extensions = "txt";
		DateTime dateTime = DateTime.Now;
		string timeStamp = dateTime.ToString("yyyyMMdd-HHmmss");
		string path = FileBrowser.SaveFile(TextResources.GetValue("filebrowser_select_save_location"), string.Empty, "CGStats_" + timeStamp, extensions);

		if (path.Length == 0) 
		{
			ShowPopUp(TextResources.GetValue("popup_no_file_selected"));
			Debug.Log(TextResources.GetValue("popup_no_file_selected"));
			return;
		} else {
			ShowPopUp(TextResources.GetValue("popup_save_file") + path);
			Debug.Log(TextResources.GetValue("popup_save_file") + path);
		}

		StringBuilder sb = new StringBuilder();

		sb.Append("GENERATION\t" + MyUIPrinter.GenerationText.text + "\n");
		sb.Append("MAP\t" + TrackNumber + "\n");
		sb.Append("NUMBER OF CARS\t" + CarCount + "\n");
		sb.Append("SELECTION METHOD\t" + SelectionMethod + "\t(0: Tournament, 1: Top 50, 2: Tournament + 20% random)\n");
		sb.Append("MUTATION POSSIBILITY\t" + MutationChance + "%\n");
		sb.Append("MUTATION RATE\t" + MutationRate + "%\n");
		sb.Append("NUMBER OF LAYERS\t" + LayersCount + "\n");
		sb.Append("NEURON PER LAYER\t" + NeuronPerLayerCount + "\n");
		sb.Append("NAVIGATOR\t" + Navigator + "\n");
		sb.Append("\n== MAX FITNESS ==\n");

		foreach (var item in MaxFitness)
		{
			sb.Append(item.ToString("F12", CultureInfo.CreateSpecificCulture("hu-HU")) + "\n");
		}

		sb.Append("\n== MEDIAN FITNESS ==\n");

		foreach (var item in MedianFitness)
		{
			sb.Append(item.ToString("F12", CultureInfo.CreateSpecificCulture("hu-HU")) + "\n");
		}

		using (StreamWriter file = new StreamWriter(path, true))
		{
			file.Write(sb.ToString());
		}
	}

	/// <summary>
	/// Az időket kezeli, meghívja az ellenőrző/fagyasztó metódusokat
	/// amikor letelnek az időkorlátok.
	/// </summary>
	private void ManageTime()
	{
		GlobalTimeLeft -= Time.deltaTime;
		FreezeTimeLeft -= Time.deltaTime;

		// Ha letelik a globális idő
		if (GlobalTimeLeft <= 0)
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
				PlayerCar.transform.gameObject.GetComponent<CarController>().Freeze();
			}

			// Az időt visszaállítja, így kezdődhet elölről.
			GlobalTimeLeft = globalTimeOut;
		}

		if (FreezeTimeLeft <= 0)
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
					AliveCount--;
				}
			}

			// Az időt visszaállítja, így kezdődhet elölről.
			FreezeTimeLeft = freezeTimeOut;
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
		if (!isAlive || carID == -1) return;
		carRigidbody.isKinematic = true;
		AliveCount--;
	}

	/// <summary>
	/// Visszaállítja az időket + mennyi autó van életben
	/// </summary>
	public void SetBackTimes()
	{
		AliveCount = CarCount;
		FreezeTimeLeft = freezeTimeOut;
		GlobalTimeLeft = globalTimeOut;
	}

	/// <summary>
	/// Visszaadja a legmagasabb fitness értékkel rendelkező
	/// életben lévő autó ID-jét.
	/// </summary>
	/// <returns>A legjobb autó ID-jét adja vissza.</returns>
	private int GetBestId()
	{
		int bestId = 0;
		float maxFitness = float.MinValue;

		for (int i = 0; i < CarCount; i++)
		{
			if (Cars[i].IsAlive)
			{
				if (maxFitness < Cars[i].Fitness)
				{
					maxFitness = Cars[i].Fitness;
					bestId = Cars[i].Id;
				}
			}
		}

		return bestId;
	}

	private void ControlMenu()
	{
		// Ha az ESC billentyűt lenyomták
		if (Input.GetButtonDown("Cancel"))
		{
			// Ha már a játékba belépett, előhozhatja az in-game menüt
			if (InGame)
			{
				// Ha aktív volt, eltünteti, ha nem volt aktív, előhozza
				master.inGameMenu.SetActive(!master.inGameMenu.activeSelf);
			}
		}

		// Ha a J billentyűt lenyomták
		if (Input.GetButtonDown("JoinDisconnect"))
		{
			// Ha jelenleg nem irányítja az autót a játékos 
			if (!ManualControl)
			{
				JoinGame();
			}
			else
			{
				DisconnectGame();
			}
		}


		if (Input.anyKeyDown)
		{
			if (!InGame)
			{
				if (Input.GetKeyDown(cheatCode[cheatIndex]))
				{
					cheatIndex++;
				}
				else
				{
					cheatIndex = 0;
				}
			}
		}

		if (cheatIndex != cheatCode.Length) return;

		cheatIndex = 0;
		master.CreateDemoSave = !master.CreateDemoSave;
		popUpText = master.CreateDemoSave ? TextResources.GetValue("popup_demo_enabled") : TextResources.GetValue("popup_demo_disabled");
		ShowPopUp(popUpText);

		Debug.Log(popUpText);
	}

	public void ShowPopUp(string message)
	{
		master.popUp.ShowPopUp(message);
	}

}

