using UnityEngine;

public abstract class GeneticAlgorithm : MonoBehaviour
{

	private UIPrinter myUIPrinter;

	public int PopulationSize { get; set; }

	public Stat[] stats;

	#region Genetic algorithm settings
	public float MutationChance { get; set; }
	public float MutationRate { get; set; }
	#endregion

	public int GenerationCount = 0;
	protected NeuralNetwork[] carNetworks;

	// Ebben a tömbben tárolja a szelekciókor létrejövő párokat.
	// Az első index a létrejövő pár sorszámát jelöli (ahány új autó kell)
	// A második index a bal (0) és jobb (1) szülőket jelöli.
	protected int[][] carPairs;

	// 4D tömbben tárolja az összes autó neurális hálójának értékeit,
	// mert rekombinációkor az eredeti értékekkel kell dolgozni.
	public double[][][][] SavedCarNetworks;



	void Awake()
	{
		PopulationSize = Master.Instance.Manager.CarCount;
		MutationChance = Master.Instance.Manager.MutationChance; // 30-70 int %
		MutationRate = Master.Instance.Manager.MutationRate;   // 2-4 float %

	}

	void Start()
	{
		carNetworks = new NeuralNetwork[PopulationSize];

		carPairs = new int[PopulationSize][];
		for (int i = 0; i < carPairs.Length; i++)
		{
			carPairs[i] = new int[2];
		}

		stats = new Stat[PopulationSize];
		for (int i = 0; i < stats.Length; i++)
		{
			stats[i] = new Stat();
		}

	}

	void FixedUpdate()
	{

		// Ha minden autó megfagyott, jöhet az új generáció
		if (Master.Instance.Manager.AliveCount <= 0)
		{
			// Elmenti az összes autó neurális hálóját
			SaveNeuralNetworks();

			if (Master.Instance.Manager.wasItALoad)
			{
				GenerationCount = Master.Instance.Manager.Save.GenerationCount;
				Master.Instance.Manager.wasItALoad = false;
			}

			// Rendezi az autókat fitness értékük szerint csökkenő sorrendben
			SortCarsByFitness();

			// Kiszámolja a maximum és a medián fitnessét az autóknak
			CalculateStats();

			// Kiválasztja a következő generáció egyedeinek szüleit
			Selection();

			// A kiválasztott párokból létrehoz új egyedeket
			RecombineAndMutate();

			// Respawnolja az új egyedeket
			RespawnCars();

		}

	}

	protected void RespawnCars()
	{
		// Respawnolja az összes autót 
		for (int i = 0; i < PopulationSize; i++)
		{
			Master.Instance.Manager.Cars[i].PrevFitness = 0;
			Master.Instance.Manager.Cars[i].IsAlive = true;
			Master.Instance.Manager.SpawnFromPool(
				Master.Instance.Manager.transform.position,
				Master.Instance.Manager.transform.rotation);
		}

		// Ha a player játszik, a piros autót is respawnolja
		if (Master.Instance.Manager.ManualControl)
		{
			Master.Instance.Manager.SpawnPlayerCar(
				Master.Instance.Manager.transform.position,
				Master.Instance.Manager.transform.rotation);
		}

		Master.Instance.Manager.SetBackTimes();
		// Növeli a generáció számlálót
		GenerationCount++;
		Master.Instance.Manager.myUIPrinter.GenerationCount = GenerationCount;
	}

	/// <summary>
	/// Inicilizálja a savedCarNetwork tömböt, melyben a neurális hálók vannak tárolva.
	/// </summary>
	public void InitSavedCarNetwork()
	{
		for (int i = 0; i < PopulationSize; i++)
		{
			carNetworks[i] = Master.Instance.Manager.Cars[i].NeuralNetwork;
		}

		SavedCarNetworks = new double[PopulationSize][][][];

		for (int i = 0; i < SavedCarNetworks.Length; i++)
		{
			SavedCarNetworks[i] = new double[carNetworks[i].NeuronLayers.Length][][];
		}
		for (int i = 0; i < SavedCarNetworks.Length; i++)
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++)
			{
				SavedCarNetworks[i][j] = new double[carNetworks[i].NeuronLayers[j].NeuronWeights.Length][];
			}
		}
		for (int i = 0; i < SavedCarNetworks.Length; i++)
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++)
			{
				for (int k = 0; k < SavedCarNetworks[i][j].Length; k++)
				{
					SavedCarNetworks[i][j][k] = new double[carNetworks[i].NeuronLayers[j].NeuronWeights[k].Length];
				}
			}
		}
	}

	/// <summary>
	/// Elmenti a neurális háló adatait a savedCarNetwork tömbbe
	/// </summary>
	public void SaveNeuralNetworks()
	{
		if (GenerationCount <= 1)
		{
			InitSavedCarNetwork();
		}


		for (int i = 0; i < SavedCarNetworks.Length; i++)    // melyik autó
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < SavedCarNetworks[i][j].Length; k++) // melyik neuron
				{
					for (int l = 0; l < SavedCarNetworks[i][j][k].Length; l++) // melyik súlya
					{
						SavedCarNetworks[i][j][k][l] = carNetworks[i].NeuronLayers[j].NeuronWeights[k][l];
					}
				}
			}
		}
	}

	/// <summary>
	/// Csökkenő sorrendbe rendezi az autók fitness értékeit.
	/// Beszúró rendezést használ.
	/// </summary>
	protected void SortCarsByFitness()
	{
		// Először összegyűjti az adatokat (ID + hozzá tartozó fitness) ...
		for (int i = 0; i < PopulationSize; i++)
		{
			stats[i].ID = Master.Instance.Manager.Cars[i].ID;
			stats[i].Fitness = Master.Instance.Manager.Cars[i].Fitness;
		}

		//  ... majd rendezi a stats tömböt
		for (int i = 0; i < PopulationSize - 1; i++)
		{
			for (int j = i + 1; j > 0; j--)
			{
				if (stats[j - 1].Fitness < stats[j].Fitness)
				{
					Stat temp = stats[j - 1];
					stats[j - 1] = stats[j];
					stats[j] = temp;
				}
			}
		}

	}


	protected abstract void Selection();


	protected virtual void RecombineAndMutate()
	{
		int index;
		float mutation;
		float mutationRateMinimum = (100 - MutationRate) / 100;
		float mutationRateMaximum = (100 + MutationRate) / 100;

		for (int i = 0; i < SavedCarNetworks.Length; i++)    // melyik autó
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < SavedCarNetworks[i][j].Length; k++) // melyik neuron
				{
					for (int l = 0; l < SavedCarNetworks[i][j][k].Length; l++) // melyik súlya
					{
						if (i == stats[0].ID)
						{
							carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								SavedCarNetworks[i][j][k][l];
						}
						else
						{

							mutation = Random.Range(mutationRateMinimum, mutationRateMaximum);
							// 50% eséllyel örököl az egyik szülőtől.
							// carPairs[i] a két szülő indexét tartalmazza
							index = carPairs[i][Random.Range(0, 2)];

							// A MutationChance értékétől függően változik a mutáció valószínűsége
							if (Random.Range(1, 100) <= MutationChance)
							{
								carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								SavedCarNetworks[index][j][k][l] * mutation;
							}
							else
							{
								carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
									SavedCarNetworks[index][j][k][l];
							}
						}
					}
				}
			}
		}


	}

	public void CalculateStats()
	{
		double max = double.MinValue;
		double median = double.MinValue;

		for (int i = 0; i < PopulationSize; i++)
		{
			if (max < Master.Instance.Manager.Cars[i].Fitness)
			{
				max = Master.Instance.Manager.Cars[i].Fitness;
			}
		}

		// A stats tömb sorba lesz rendezve,
		// így onnan tudjuk, hogy melyik a középső autó
		median = Master.Instance.Manager.Cars[
			stats[PopulationSize / 2].ID].Fitness;


		Master.Instance.Manager.maxFitness.Add(max);
		Master.Instance.Manager.medianFitness.Add(median);

	}


}
