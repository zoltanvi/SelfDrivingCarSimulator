using System;
using UnityEngine;


public abstract class GeneticAlgorithm : MonoBehaviour
{
	protected struct Stat : IComparable
	{
		public int Id;
		public float Fitness;

		public int CompareTo(object obj)
		{
			if (obj == null) return 1;  // this is greater than obj

			if (!(obj is Stat)) throw new ArgumentException("Object is not a Stat!");
			Stat other = (Stat)obj;
			return this.Fitness.CompareTo(other.Fitness);
		}
	}

	private UIPrinter myUIPrinter;

	public int PopulationSize { get; set; }

	protected Stat[] Stats;

	#region Genetic algorithm settings
	public float MutationChance { get; set; }
	public float MutationRate { get; set; }
	#endregion

	public int GenerationCount = 0;
	protected NeuralNetwork[] CarNetworks;

	// Ebben a tömbben tárolja a szelekciókor létrejövő párokat.
	// Az első index a létrejövő pár sorszámát jelöli (ahány új autó kell)
	// A második index a bal (0) és jobb (1) szülőket jelöli.
	protected int[][] carPairs;

	// 4D tömbben tárolja az összes autó neurális hálójának értékeit,
	// mert rekombinációkor az eredeti értékekkel kell dolgozni.
	public float[][][][] SavedCarNetworks;

	private Manager manager;

	private void Awake()
	{
		manager = Master.Instance.Manager;
		PopulationSize = manager.CarCount;
		MutationChance = manager.MutationChance; // 30-70 int %
		MutationRate = manager.MutationRate;   // 2-4 float %

	}

	private void Start()
	{
		CarNetworks = new NeuralNetwork[PopulationSize];

		carPairs = new int[PopulationSize][];
		int carPairsLength = carPairs.Length;
		for (int i = 0; i < carPairsLength; i++)
		{
			carPairs[i] = new int[2];
		}

		Stats = new Stat[PopulationSize];
		int statsLength = Stats.Length;
		for (int i = 0; i < statsLength; i++)
		{
			Stats[i] = new Stat();
		}

	}

	private void FixedUpdate()
	{

		// Ha minden autó megfagyott, jöhet az új generáció
		if (manager.AliveCount > 0) return;

		// Elmenti az összes autó neurális hálóját
		SaveNeuralNetworks();

		if (manager.WasItALoad)
		{
			GenerationCount = manager.Save.GenerationCount;
			manager.WasItALoad = false;
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

	protected void RespawnCars()
	{
		// Respawnolja az összes autót 
		for (int i = 0; i < PopulationSize; i++)
		{
			manager.Cars[i].PrevFitness = 0;
			manager.Cars[i].IsAlive = true;
			manager.SpawnFromPool(
				manager.transform.position,
				manager.transform.rotation);
		}

		// Ha a player játszik, a piros autót is respawnolja
		if (manager.ManualControl)
		{
			manager.SpawnPlayerCar(
				manager.transform.position,
				manager.transform.rotation);
		}

		manager.SetBackTimes();
		// Növeli a generáció számlálót
		GenerationCount++;
		manager.MyUIPrinter.GenerationCount = GenerationCount;
	}

	/// <summary>
	/// Inicilizálja a savedCarNetwork tömböt, melyben a neurális hálók vannak tárolva.
	/// </summary>
	public void InitSavedCarNetwork()
	{
		for (int i = 0; i < PopulationSize; i++)
		{
			CarNetworks[i] = manager.Cars[i].NeuralNetwork;
		}

		SavedCarNetworks = new float[PopulationSize][][][];

		for (int i = 0; i < SavedCarNetworks.Length; i++)
		{
			SavedCarNetworks[i] = new float[CarNetworks[i].NeuronLayers.Length][][];
		}
		for (int i = 0; i < SavedCarNetworks.Length; i++)
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++)
			{
				SavedCarNetworks[i][j] = new float[CarNetworks[i].NeuronLayers[j].NeuronWeights.Length][];
			}
		}
		for (int i = 0; i < SavedCarNetworks.Length; i++)
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++)
			{
				for (int k = 0; k < SavedCarNetworks[i][j].Length; k++)
				{
					SavedCarNetworks[i][j][k] = new float[CarNetworks[i].NeuronLayers[j].NeuronWeights[k].Length];
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
						SavedCarNetworks[i][j][k][l] = CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l];
					}
				}
			}
		}
	}

	/// <summary>
	/// Csökkenő sorrendbe rendezi az autók fitness értékeit.
	/// </summary>
	protected void SortCarsByFitness()
	{
		// Először összegyűjti az adatokat (ID + hozzá tartozó fitness) ...
		for (int i = 0; i < PopulationSize; i++)
		{
			Stats[i].Id = manager.Cars[i].Id;
			Stats[i].Fitness = manager.Cars[i].Fitness;
		}

		//  ... majd rendezi a stats tömböt csökkenő sorrendbe
		Array.Sort(Stats);
		Array.Reverse(Stats);
	}

	protected abstract void Selection();

	protected virtual void RecombineAndMutate()
	{
		float mutationRateMinimum = (100 - MutationRate) / 100;
		float mutationRateMaximum = (100 + MutationRate) / 100;

		int savedCarNetworksLength = SavedCarNetworks.Length;
		for (int i = 0; i < savedCarNetworksLength; i++)    // melyik autó
		{
			int savedCarNetworksILength = SavedCarNetworks[i].Length;
			for (int j = 0; j < savedCarNetworksILength; j++) // melyik neuronréteg
			{
				int savedCarNetworksIJLength = SavedCarNetworks[i][j].Length;
				for (int k = 0; k < savedCarNetworksIJLength; k++) // melyik neuron
				{
					int savedCarNetworksIJKLength = SavedCarNetworks[i][j][k].Length;
					for (int l = 0; l < savedCarNetworksIJKLength; l++) // melyik súlya
					{
						if (i == Stats[0].Id)
						{
							CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								SavedCarNetworks[i][j][k][l];
						}
						else
						{
							float mutation = RandomHelper.NextFloat(mutationRateMinimum, mutationRateMaximum);
							// 50% eséllyel örököl az egyik szülőtől.
							// carPairs[i] a két szülő indexét tartalmazza
							int index = carPairs[i][RandomHelper.NextInt(0, 1)];

							// A MutationChance értékétől függően változik a mutáció valószínűsége
							if (RandomHelper.NextInt(0, 100) <= MutationChance)
							{
								CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
									SavedCarNetworks[index][j][k][l] * mutation;
							}
							else
							{
								CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
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

		float max = float.MinValue;

		for (int i = 0; i < PopulationSize; i++)
		{
			if (max < manager.Cars[i].Fitness)
			{
				max = manager.Cars[i].Fitness;
			}
		}

		// A stats tömb sorba lesz rendezve,
		// így onnan tudjuk, hogy melyik a középső autó
		float median = manager.Cars[
			Stats[PopulationSize / 2].Id].Fitness;


		manager.MaxFitness.Add(max);
		manager.MedianFitness.Add(median);

	}

}

