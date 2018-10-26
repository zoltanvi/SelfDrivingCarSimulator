using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithmWorstRandom : GeneticAlgorithm
{

	// A tournament során ennyi autó "versenyzik" egyszerre egymással
	private int selectionPressure = 3;
	private int top80;

	protected override void Selection()
	{

		// A kiválasztott autó ID-ket tárolja egy körig
		List<int> picked = new List<int>();

		int paired = 0;

		// Az első szülő minden párnál full random
		for (int i = 0; i < PopulationSize; i++)
		{
			carPairs[i][0] = Random.Range(0, PopulationSize);
		}


		// Amíg meg nincs meg az összes pár, új tournament
		while (paired < PopulationSize)
		{
			#region Jelenlegi tournament inicializálása
			List<int> tournament = new List<int>();
			for (int i = 0; i < PopulationSize; i++)
			{
				tournament.Add(i);
			}
			#endregion

			// Amíg van elég versenyző a tournamenten belül (és még kell pár), versenyzők kiválasztása
			while (tournament.Count >= selectionPressure && paired < PopulationSize)
			{
				// A kiválasztottak kiürítése
				picked.Clear();

				// Amíg meg nincs mindegyik versenyző
				while (picked.Count != selectionPressure)
				{
					int current = tournament[Random.Range(0, tournament.Count)];
					if (!picked.Contains(current))
					{
						picked.Add(current);
						tournament.Remove(current);
					}
				}

				// Párosítás
				carPairs[paired][1] = GetTournamentBestIndex(picked);
				paired++;
			}
		}

#if UNITY_EDITOR
		string tmp = "";
		for (int i = 0; i < carPairs.Length; i++)
		{
			tmp += carPairs[i][0] + " :: " + carPairs[i][1] + "\n";
		}
		Debug.Log(tmp);
#endif
	}

	private int GetTournamentBestIndex(List<int> picked)
	{
		int bestIndex = int.MinValue;
		double highestFitness = double.MinValue;
		foreach (var thispicked in picked)
		{
			double currentFitness = 0;
			foreach (var stat in Stats)
			{
				if (stat.Id == thispicked)
				{
					currentFitness = stat.Fitness;
				}
			}
			if (currentFitness > highestFitness)
			{
				highestFitness = currentFitness;
				bestIndex = thispicked;
			}
		}
		return bestIndex;
	}

	protected override void RecombineAndMutate()
	{
		int index;
		float mutation;
		float mutationRateMinimum = (100 - MutationRate) / 100;
		float mutationRateMaximum = (100 + MutationRate) / 100;
		top80 = (int)(PopulationSize * 0.8f);

		for (int i = 0; i < SavedCarNetworks.Length; i++)    // melyik autó
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < SavedCarNetworks[i][j].Length; k++) // melyik neuron
				{
					for (int l = 0; l < SavedCarNetworks[i][j][k].Length; l++) // melyik súlya
					{
						if (i == Stats[0].Id)
						{
							CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								SavedCarNetworks[i][j][k][l];
						}
						else if (i >= top80)
						{
							// Az autók 20%-a újra lesz randomolva minden körben.
							CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] = Random.Range(-1f, 1f);
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




}
