using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using Crosstales.FB;
using System;
using System.Globalization;
using UnityEngine.PostProcessing;

public class Manager : MonoBehaviour
{
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
    public double Bias { get; set; }
    public bool Navigator { get; set; }
    public bool DemoMode { get; set; }
    [HideInInspector] public bool GotOptionValues = false;
    [HideInInspector] public bool inGame = false;
    [HideInInspector] public bool ManualControl = false;
    [HideInInspector] public bool wasItALoad = false;
    [HideInInspector] public UIPrinter myUIPrinter;
    [HideInInspector] public List<double> maxFitness = new List<double>();
    [HideInInspector] public List<double> medianFitness = new List<double>();
    [HideInInspector] public int AliveCount { get; set; }
    [HideInInspector] public int bestCarID = 0;
    [HideInInspector] public float freezeTimeLeft = freezeTimeOut;
    [HideInInspector] public float globalTimeLeft = globalTimeOut;
    [HideInInspector] public GameObject CurrentTrack;
    [HideInInspector] public GameObject CurrentWaypoint;
    [HideInInspector] public double PlayerFitness { get; set; }
    [HideInInspector] public bool isPlayerAlive = false;
    [HideInInspector] public GameObject rayHolderRoot;
    [HideInInspector] public GameObject carHolderRoot;
    [HideInInspector] public GameObject playerCar;
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

    #endregion


    void Start()
    {
        myUIPrinter = Master.Instance.UIStats.GetComponent<UIPrinter>();

        Save = new GameSave();
        transparentShader = Shader.Find("Standard (Specular setup)");
        standardShader = Shader.Find("Standard");
        visibleColor = new Color(1, 1, 1, 1.0f);
        transparentColor = new Color(1, 1, 1, 0.0f);
        cheatCode = new string[] { "n", "e", "r", "f" };
        cheatIndex = 0;


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

            // Ha játszik a játékos és még életben van, őt követi a kamera..
            // Különben a legjobb élő autót
            if (ManualControl && isPlayerAlive)
            {
                Master.Instance.cameraDrone.CameraTarget = playerCar.transform;
                myUIPrinter.FitnessValue = PlayerFitness;
                myUIPrinter.ConsoleMessage = "";
            }
            else
            {
                bestCarID = GetBestID();
                myUIPrinter.FitnessValue = Cars[bestCarID].Fitness;
                Master.Instance.cameraDrone.CameraTarget = Cars[bestCarID].Transform;

                myUIPrinter.ConsoleMessage = "";
                for (int i = 0; i < Cars[bestCarID].Inputs.Length; i++)
                {
                    myUIPrinter.ConsoleMessage += string.Format("> {0:0.000}\n", Cars[bestCarID].Inputs[i]);
                }
            }

        }

    }


    /// <summary>
    /// Létrehozza az autókat a poolba. Ezek még nem aktív autók!
    /// Aktiválni EGY autót a SpawnFromPool metódussal lehet.
    /// Aktiválni AZ ÖSSZES autót a SpawnCars metódussal lehet.
    /// </summary>
    void InstantiateCars()
    {
        carPool = new Queue<GameObject>();
        rayHolderRoot = new GameObject("RayHolderDeletable");
        carHolderRoot = new GameObject("CarHolderDeletable");
        for (int i = 0; i < CarCount; i++)
        {
            GameObject obj = Instantiate(Master.Instance.blueCarPrefab, transform.position, transform.rotation);

            obj.SetActive(false);
            carPool.Enqueue(obj);

            Cars[i].GameObject = obj;
            Cars[i].CarController = obj.GetComponent<CarController>();
            Cars[i].NeuralNetwork = obj.GetComponent<NeuralNetwork>();
            Cars[i].CarController.ID = i;
            Cars[i].Transform = obj.transform;
            Cars[i].Transform.parent = carHolderRoot.transform;
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
        playerCar = Instantiate(Master.Instance.redCarPrefab, transform.position, transform.rotation);

        playerCarController = playerCar.GetComponent<CarController>();
        playerCarController.IsPlayerControlled = true;
        playerCarController.ID = -1;
        playerCar.transform.name = "PlayersCarDeletable";
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
        if (!playerFirstStart)
        {
            playerCar.GetComponent<FitnessMeter>().Reset();
        }
        return playerCar;
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
        isPlayerAlive = false;
        playerCar.SetActive(false);
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

        Component[] comps;

        if (!ManualControl)
        {
            for (int i = 0; i < CarCount; i++)
            {
                comps = Cars[i].Transform.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in comps)
                {
                    renderer.material.shader = standardShader;
                    renderer.material.SetColor("_Color", visibleColor);
                    renderer.material.SetFloat("_Mode", 0);

                    renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    renderer.material.SetInt("_ZWrite", 1);
                    renderer.material.DisableKeyword("_ALPHATEST_ON");
                    renderer.material.DisableKeyword("_ALPHABLEND_ON");
                    renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer.material.renderQueue = -1;
                }

            }
        }
        else
        {

            for (int i = 0; i < CarCount; i++)
            {
                comps = Cars[i].Transform.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in comps)
                {
                    renderer.material.shader = transparentShader;
                    renderer.material.SetColor("_Color", transparentColor);
                    renderer.material.SetFloat("_Mode", 3);

                    renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    renderer.material.SetInt("_ZWrite", 0);
                    renderer.material.DisableKeyword("_ALPHATEST_ON");
                    renderer.material.DisableKeyword("_ALPHABLEND_ON");
                    renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer.material.renderQueue = 3000;
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
            wasItALoad = true;
        }

        InitGame();
        PlayGame();
    }

    public void InitGame()
    {
        Bias = 1.0;
        InitTrack();
        InitCars();
        InitGenetic();


        AliveCount = CarCount;
        Master.Instance.cameraDrone.enabled = false;
        Master.Instance.Camera.GetComponent<PostProcessingBehaviour>().enabled = true;

    }

    public void PlayGame()
    {
        InstantiateCars();
        // A player autóját is példányosítja a játék indulásakor
        InstantiatePlayerCar();
        SpawnCars();

        Master.Instance.UIStats.SetActive(true);

        Master.Instance.cameraDrone.CameraTarget = Cars[0].Transform;
        Master.Instance.cameraDrone.enabled = true;
        // Első spawnolás megtörtént
        firstStart = false;
        inGame = true;

    }

    /// <summary>
    /// A genetikus algoritmust inicializálja, attól függően, hogy
    /// mi volt az options menüben kiválasztva.
    /// </summary>
    void InitGenetic()
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
            // Tournament selection
            default:
                GAGameObject.AddComponent<GeneticAlgorithmTournament>();
                GA = GAGameObject.GetComponent<GeneticAlgorithmTournament>();
                break;
        }

    }

    void InitTrack()
    {
        switch (TrackNumber)
        {
            case 0:
                Master.Instance.minimapCamera.transform.position = new Vector3(-34.0f, 7.0f, 22.8f);
                break;

            case 1:
                Master.Instance.minimapCamera.transform.position = new Vector3(-1.0f, 7.0f, -2.5f);
                break;

            case 2:
                Master.Instance.minimapCamera.transform.position = new Vector3(10.0f, 7.0f, 5.0f);
                break;

            default:
                Master.Instance.minimapCamera.transform.position = new Vector3(-34.0f, 7.0f, 22.8f);
                break;
        }


        CurrentTrack = Instantiate(Master.Instance.TrackPrefabs[TrackNumber], transform.position, transform.rotation);
        CurrentWaypoint = Instantiate(Master.Instance.WayPointPrefabs[TrackNumber], transform.position, transform.rotation);
        CurrentTrack.transform.name = "TrackDeletable";
        CurrentWaypoint.transform.name = "WaypointDeletable";
        DontDestroyOnLoad(CurrentTrack);
        DontDestroyOnLoad(CurrentWaypoint);
        CurrentTrack.SetActive(true);
        CurrentWaypoint.SetActive(true);
    }

    void InitCars()
    {
        // Inputs tömb mérete nagyobb, ha az autó inputként megkapja a sarkokat is!!
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
                ID = i,
                Fitness = 0,
                Inputs = new double[inputCount],
                PrevFitness = 0
            };
        }
    }

    public void LoadGame()
    {
        string extensions = "SAVE";
        string path = FileBrowser.OpenSingleFile("Select a saved game to load", "", extensions);
        Debug.Log("Selected file: " + path);

        if (Path.GetExtension(path).Equals(".SAVE"))
        {
            if (File.Exists(path))
            {
                FileStream file;

                using (file = File.Open(path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Save = (GameSave)bf.Deserialize(file);
                }

                SelectionMethod = Save.SelectionMethod;
                MutationChance = Save.MutationChance;
                MutationRate = Save.MutationRate;
                CarCount = Save.CarCount;
                LayersCount = Save.LayersCount;
                NeuronPerLayerCount = Save.NeuronPerLayerCount;
                Navigator = Save.Navigator;
                //TrackNumber = Save.TrackNumber;
                medianFitness = Save.medianFitness;
                maxFitness = Save.maxFitness;

                GotOptionValues = true;
                wasItALoad = true;

                Master.Instance.StartNewGame();
                Master.Instance.mainMenuCanvas.SetActive(false);
                Master.Instance.bgLights.SetActive(false);
            }
        }
        else if (path.Length == 0)
        {
            Debug.Log("No file selected.");
        }
        else
        {
            Debug.LogError("You tried to open a wrong file!");
        }

    }

    public void SaveGame()
    {
        string extensions = "SAVE";
        DateTime dateTime = DateTime.Now;
        string timeStamp = dateTime.ToString("yyyy-MM-dd__HH-mm-ss");
        string path = FileBrowser.SaveFile("Select the save location", "", "CGSave_" + timeStamp, extensions);
        Debug.Log("Save file: " + path);

        if (path.Length != 0)
        {
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

            if (Master.Instance.CreateDemoSave)
            {
                Save.GenerationCount = 0;
                Save.maxFitness = new List<double>();
                Save.medianFitness = new List<double>();
            }
            else
            {
                Save.GenerationCount = GA.GenerationCount;
                Save.maxFitness = maxFitness;
                Save.medianFitness = medianFitness;
            }


            FileStream file;
            using (file = File.Create(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, Save);
            }
        }


    }


    public void SaveStats()
    {
        string extensions = "txt";
        DateTime dateTime = DateTime.Now;
        string timeStamp = dateTime.ToString("yyyy-MM-dd__HH-mm-ss");
        string path = FileBrowser.SaveFile("Select the save location", "", "CGStats_" + timeStamp, extensions);
        Debug.Log("Save file: " + path);

        if (path.Length != 0)
        {
            string tmp =
                "GENERATION\t" + myUIPrinter.generationText.text + "\n" +
                "MAP\t" + TrackNumber + "\n" +
                "NUMBER OF CARS\t" + CarCount + "\n" +
                "SELECTION METHOD\t" + SelectionMethod + "\t(0: Tournament, 1: Top 50, 2: Tournament + 20% random)\n" +
                "MUTATION POSSIBILITY\t" + MutationChance + "%\n" +
                "MUTATION RATE\t" + MutationRate + "%\n" +
                "NUMBER OF LAYERS\t" + LayersCount + "\n" +
                "NEURON PER LAYER\t" + NeuronPerLayerCount + "\n" +
                "NAVIGATOR\t" + Navigator + "\n";

            tmp += "\n== MAX FITNESS ==\n";
            foreach (double item in maxFitness)
            {
                tmp += item.ToString("F12", CultureInfo.CreateSpecificCulture("hu-HU")) + "\n";
            }

            tmp += "\n== MEDIAN FITNESS ==\n";
            foreach (double item in medianFitness)
            {
                tmp += item.ToString("F12", CultureInfo.CreateSpecificCulture("hu-HU")) + "\n";
            }

            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.Write(tmp);
            }

        }
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
                #region Súlyok és fitness értékek fájlba írása
                //if (LogNetworkData)
                //{
                //	NeuralNetwork tmp2 = Cars[carID].Transform.gameObject.GetComponent<NeuralNetwork>();


                //	string carNNWeights = carID + ". car:\n";
                //	for (int i = 0; i < tmp2.NeuronLayers.Length; i++)
                //	{
                //		carNNWeights += (i + 1) + ". layer: \n";
                //		for (int k = 0; k < tmp2.NeuronLayers[i].NeuronWeights.Length; k++)
                //		{
                //			for (int j = 0; j < tmp2.NeuronLayers[i].NeuronWeights[0].Length; j++)
                //			{
                //				string tmp = string.Format("{0,10}", tmp2.NeuronLayers[i].NeuronWeights[k][j]);
                //				carNNWeights += tmp + "\t";
                //			}
                //			carNNWeights += "\n";
                //		}
                //		carNNWeights += "\n";
                //	}


                //	GameLogger.WriteData(carNNWeights);
                //	string dataText = "Maximum fitness: " + Cars[carID].Fitness + "\n\n";
                //	GameLogger.WriteData(dataText);

                //}
                #endregion
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
            // Ha már a játékba belépett, előhozhatja az in-game menüt
            if (inGame)
            {
                // Ha aktív volt, eltünteti, ha nem volt aktív, előhozza
                if (Master.Instance.inGameMenu.activeSelf)
                {
                    Master.Instance.inGameMenu.SetActive(false);
                }
                else
                {
                    Master.Instance.inGameMenu.SetActive(true);
                }
            }
        }

        // Ha a J billentyűt lenyomták
        if(Input.GetButtonDown("JoinDisconnect"))
        {
            // Ha jelenleg nem irányítja az autót a játékos 
            if(!ManualControl){
                JoinGame();
            } else {
                DisconnectGame();
            }
        }


        if (Input.anyKeyDown)
        {
            if (!inGame)
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

        if (cheatIndex == cheatCode.Length)
        {
            cheatIndex = 0;
            Master.Instance.CreateDemoSave = !Master.Instance.CreateDemoSave;
            Instantiate(Master.Instance.popUp);

            print("Cheat unlocked!");
        }

    }


}
