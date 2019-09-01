using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;


public class Master : MonoBehaviour
{

	public static Master Instance = null;
	[HideInInspector] public GameObject ManagerGO;
	[HideInInspector] public Manager Manager;
	public MenuController2 MenuController;

	[Header("Car prefabs")]
	public GameObject BlueCarPrefab;
	public GameObject RedCarPrefab;
	[Header("If you want to create a demo save file")]
	[Tooltip("If it is checked, the save file won't contains the statistics and the generation counter.")]
	public bool CreateDemoSave = false;

	[Header("Component references")]
	public GameObject UIStats;
	public GameObject inGameMenu;
	public GameObject mainMenuCanvas;
	public GameObject bgLights;
	public GameObject minimapCamera;
	public CameraController cameraController;
	public GameObject Camera;
	public PopUpText popUp;

	[Header("Track prefabs")]
	public GameObject[] TrackPrefabs;
	public GameObject[] WayPointPrefabs;


    public int CurrentConfigId { get; set; }
    public int CurrentEditConfigId { get; set; }
    public List<Configuration> Configurations;
    public int SimulationCount { get; set; }

    public string DefaultSaveLocation = string.Empty;

    public const int MAX_CONFIGURATIONS = 15;

    public Configuration EditConfiguration
    {
        get
        {
            if(CurrentEditConfigId > Configurations.Count - 1)
            {
                Debug.Log($"Configurations count was: {Configurations.Count}");
                int needToCreate = CurrentEditConfigId - Configurations.Count + 1;

                for (int i = 0; i < needToCreate; i++)
                {
                    Configurations.Add(new Configuration());
                }
                Debug.Log($"Configurations count now is: {Configurations.Count}");
            }

            if (Configurations[CurrentEditConfigId] == null)
            {
                Configurations[CurrentEditConfigId] = new Configuration();
            }

            return Configurations[CurrentEditConfigId];
        }
    }

    public Configuration CurrentConfiguration
    {
        get
        {
            if (CurrentConfigId >= Configurations.Count)
            {
                throw new IndexOutOfRangeException("The current config ID was higher than expected!");
            }

            if (Configurations[CurrentConfigId] == null)
            {
                Configurations[CurrentConfigId] = new Configuration();
            }

            return Configurations[CurrentConfigId];
        }
        set
        {
            if (CurrentConfigId >= Configurations.Count)
            {
                throw new IndexOutOfRangeException("The current config ID was higher than expected!");
            }

            Configurations[CurrentConfigId] = value;
        }
    }

    private void Awake()
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
        Configurations = new List<Configuration> { new Configuration() };
        RandomHelper.GenerateNewSeed();
        Debug.Log("Seed beállítva!");
    }

    public void PopulateConfigsUntil(int lastIndex)
    {
        for (int i = 0; i < lastIndex; i++)
        {
            CurrentEditConfigId = i;
            PopulateCurrentConfig();
        }
    }

    public void RemoveConfigs(int startIndex)
    {
        for (int i = Configurations.Count - 1; i >= startIndex; i--)
        {
            Configurations.RemoveAt(i);
        }
    }

    private void PopulateCurrentConfig()
    {
        Configuration config = EditConfiguration;

        if (!config.IsPopulated)
        {
            // fill config with default data
            config.IsPopulated = true;
            config.CarCount = 20;
            config.LayersCount = 3;
            config.NeuronPerLayerCount = 6;
            config.SelectionMethod = 1;         // Top 50%
            config.MutationChance = 50;          // 50%
            config.MutationRate = 3;            // 3%
            config.DemoMode = false;
            config.Navigator = false;
            config.StopConditionActive = true;
            config.StopGenerationNumber = 100;
            config.TrackNumber = 0;
        }
    }

    private void Start()
	{
		ManagerGO = new GameObject("MANAGER");
		Manager = ManagerGO.AddComponent<Manager>();
		DontDestroyOnLoad(ManagerGO);

		DontDestroyOnLoad(UIStats);
		DontDestroyOnLoad(inGameMenu);
		DontDestroyOnLoad(minimapCamera);
		DontDestroyOnLoad(mainMenuCanvas);
		DontDestroyOnLoad(bgLights);
		mainMenuCanvas.SetActive(true);
		UIStats.SetActive(false);
		inGameMenu.SetActive(false);


        PopulateConfigsUntil(Configurations.Count);

        CurrentEditConfigId = 0;
    }


	public void JoinGame()
	{
		Manager.JoinGame();
	}


	public void DisconnectGame()
	{
		Manager.DisconnectGame();
	}


	public void SaveGame()
	{
		Manager.SaveGame();
	}

	public void LoadGame()
	{
		Manager.LoadGame();
	}

	public void SaveStats()
	{
		Manager.SaveStats();
	}

	public void StartNewGame(bool isLoad = false)
	{
        if (!isLoad)
        {
            InitManagerConfig();
        }

        inGameMenu.SetActive(false);
        UIStats.SetActive(true);
        mainMenuCanvas.SetActive(false);

        Manager.StartGame();
		Camera.transform.position = new Vector3(0.62f, 5.83f, -7.5f);
	}

    public void InitManagerConfig()
    {
        Manager.Configuration = CurrentConfiguration;
    } 

	/// <summary>
	/// Beállítja a szimuláció sebességet
	/// </summary>
	public void SetTimeScale(float scale = 1.0f)
	{
		Time.timeScale = scale;
	}


	public void ExitGame()
	{
#if UNITY_EDITOR
		Debug.Log("Kilépés...");
		UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
	}

	[ContextMenu("Back to menu")]
	public void BackToMenu()
	{
		Camera.transform.position = new Vector3(0.62f, 305.83f, -7.55f);
		Camera.transform.rotation = new Quaternion();
		Camera.GetComponent<PostProcessingBehaviour>().enabled = false;

		cameraController.enabled = false;
		Destroy(GameObject.Find("GeneticAlgorithmDeletable"));
		Destroy(GameObject.Find("TrackDeletable"));
		Destroy(GameObject.Find("WaypointDeletable"));
		Destroy(GameObject.Find("RayHolderDeletable"));
		Destroy(GameObject.Find("CarHolderDeletable"));
		Manager.MyUIPrinter.GenerationCount = 0;
		Destroy(Manager.PlayerCar);
		Destroy(ManagerGO);
		inGameMenu.SetActive(false);
		UIStats.SetActive(false);
		mainMenuCanvas.SetActive(true);

		ManagerGO = new GameObject("ManagerObject");
		Manager = ManagerGO.AddComponent<Manager>();
		DontDestroyOnLoad(ManagerGO);
	}

    public void StartNextSimulation()
    {
        BackToMenu();

        if (CurrentConfigId < Configurations.Count - 1)
        {
            CurrentConfigId++;
            StartNewGame();
        }
    }

    public void OnSimulationFinished()
    {
        Manager.SaveGame();
        Manager.SaveStats();
        StartNextSimulation();
    }

	public void GenerateNewSeed()
	{
		RandomHelper.GenerateNewSeed();
	}

}

