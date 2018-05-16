using UnityEngine;

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


	public bool GotOptionValues = false;

	private GameObject GAGameObject;
	private GeneticAlgorithm GA;

	[SerializeField] private GameObject[] TrackPrefabs;
	[HideInInspector] public GameObject CurrentTrack;

	[SerializeField] private CameraDrone cameraDrone;

	public Car[] Cars;
	[HideInInspector] public int AliveCount { get; set; }

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


	// A START gomb hívja meg ezt a metódust
	public void InitGame()
	{
		Debug.Log("SelectionMethod: " + SelectionMethod);
		InitTrack();
		InitGenetic();
		InitCars();

		cameraDrone.enabled = false;
	}


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

		// Teszt
		GA.RecombineAndMutate();

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


}
