using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneticAlgorithm : MonoBehaviour
{

	private UIPrinter myUIPrinter;

	public int PopulationSize { get; set; }

	protected Stat[] stats;

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
	protected double[][][][] savedCarNetwork;



	void Awake()
	{
		PopulationSize = Manager.Instance.CarCount;
		// Megfelelő SelectionMethod switch-case
		MutationChance = Manager.Instance.MutationChance; // 30-70 int %
		MutationRate = Manager.Instance.MutationRate;   // 2-4 float %

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
		if (Manager.Instance.AliveCount <= 0 && Manager.Instance.isPlayerAlive == false)
		{
			// Elmenti az összes autó neurális hálóját
			SaveNeuralNetworks();

			// Rendezi az autókat fitness értékük szerint csökkenő sorrendben
			SortCarsByFitness();

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
			Manager.Instance.Cars[i].PrevFitness = 0;
			Manager.Instance.Cars[i].IsAlive = true;
			Manager.Instance.SpawnFromPool(
				Manager.Instance.transform.position,
				Manager.Instance.transform.rotation);
		}

		// Ha a player játszik, a piros autót is respawnolja
		if (Manager.Instance.ManualControl)
		{
			Manager.Instance.SpawnPlayerCar(
				Manager.Instance.transform.position,
				Manager.Instance.transform.rotation);
		}

		Manager.Instance.SetBackTimes();
		// Növeli a generáció számlálót
		GenerationCount++;
		Manager.Instance.myUIPrinter.GenerationCount = GenerationCount;
	}

	/// <summary>
	/// Inicilizálja a savedCarNetwork tömböt, melyben a neurális hálók vannak tárolva.
	/// </summary>
	protected void InitSavedCarNetwork()
	{
		for (int i = 0; i < PopulationSize; i++)
		{
			carNetworks[i] = Manager.Instance.Cars[i].NeuralNetwork;
		}

		savedCarNetwork = new double[PopulationSize][][][];

		for (int i = 0; i < savedCarNetwork.Length; i++)
		{
			savedCarNetwork[i] = new double[carNetworks[i].NeuronLayers.Length][][];
		}
		for (int i = 0; i < savedCarNetwork.Length; i++)
		{
			for (int j = 0; j < savedCarNetwork[i].Length; j++)
			{
				savedCarNetwork[i][j] = new double[carNetworks[i].NeuronLayers[j].NeuronWeights.Length][];
			}
		}
		for (int i = 0; i < savedCarNetwork.Length; i++)
		{
			for (int j = 0; j < savedCarNetwork[i].Length; j++)
			{
				for (int k = 0; k < savedCarNetwork[i][j].Length; k++)
				{
					savedCarNetwork[i][j][k] = new double[carNetworks[i].NeuronLayers[j].NeuronWeights[k].Length];
				}
			}
		}
	}

	/// <summary>
	/// Elmenti a neurális háló adatait a savedCarNetwork tömbbe
	/// </summary>
	protected void SaveNeuralNetworks()
	{
		if (GenerationCount <= 1)
		{
			InitSavedCarNetwork();
		}


		for (int i = 0; i < savedCarNetwork.Length; i++)    // melyik autó
		{
			for (int j = 0; j < savedCarNetwork[i].Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < savedCarNetwork[i][j].Length; k++) // melyik neuron
				{
					for (int l = 0; l < savedCarNetwork[i][j][k].Length; l++) // melyik súlya
					{
						savedCarNetwork[i][j][k][l] = carNetworks[i].NeuronLayers[j].NeuronWeights[k][l];
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
			stats[i].ID = Manager.Instance.Cars[i].ID;
			stats[i].Fitness = Manager.Instance.Cars[i].Fitness;
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


	protected void RecombineAndMutate()
	{
		int index;
		float mutation;
		float mutationRateMinimum = (100 - MutationRate) / 100;
		float mutationRateMaximum = (100 + MutationRate) / 100;

		for (int i = 0; i < savedCarNetwork.Length; i++)    // melyik autó
		{
			for (int j = 0; j < savedCarNetwork[i].Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < savedCarNetwork[i][j].Length; k++) // melyik neuron
				{
					for (int l = 0; l < savedCarNetwork[i][j][k].Length; l++) // melyik súlya
					{
						if (i == stats[0].ID)
						{
							carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								savedCarNetwork[i][j][k][l];
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
								savedCarNetwork[index][j][k][l] * mutation;
							}
							else
							{
								carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
									savedCarNetwork[index][j][k][l];
							}
						}
					}
				}
			}
		}


	}



}
