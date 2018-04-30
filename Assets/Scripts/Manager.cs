using UnityEngine;

public class Manager : MonoBehaviour
{
	public static Manager Instance = null;

	public int CarCount { get; set; }
	public int SelectionMethod { get; set; }
	public int MutationChance { get; set; }
	public float MutationRate { get; set; }
	public int LayersCount { get; set; }
	public int NeuronPerLayerCount { get; set; }


	public bool GotOptionValues = false;

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
		InitGame();

	}

	void InitGame()
	{

	}

	void Update()
	{
		

	}


}
