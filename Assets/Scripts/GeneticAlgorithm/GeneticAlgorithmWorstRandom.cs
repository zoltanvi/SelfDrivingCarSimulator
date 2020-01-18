using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GeneticAlgorithmWorstRandom : GeneticAlgorithm
{
	// A tournament során ennyi autó "versenyzik" egyszerre egymással
	private int m_SelectionPressure = 3;
	private int m_Top80Percent;

	protected override void Selection()
	{
		// A kiválasztott autó ID-ket tárolja egy körig
		List<int> pickedCarIdList = new List<int>();

		int paired = 0;

		// Az első szülő minden párnál full random
		for (int i = 0; i < PopulationSize; i++)
		{
			CarPairs[i][0] = RandomHelper.NextInt(0, PopulationSize - 1);
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
			while (tournament.Count >= m_SelectionPressure && paired < PopulationSize)
			{
				// A kiválasztottak kiürítése
				pickedCarIdList.Clear();

				// Amíg meg nincs mindegyik versenyző
				while (pickedCarIdList.Count != m_SelectionPressure)
				{
					int current = tournament[RandomHelper.NextInt(0, tournament.Count - 1)];
					if (!pickedCarIdList.Contains(current))
					{
						pickedCarIdList.Add(current);
						tournament.Remove(current);
					}
				}

				// Párosítás
				CarPairs[paired][1] = GetTournamentBestIndex(pickedCarIdList);
				paired++;
			}
		}

#if UNITY_EDITOR
        StringBuilder sb = new StringBuilder();
        foreach (var carPair in CarPairs)
        {
            sb.Append($"{carPair[0]} :: {carPair[1]} \n");
        }
        Debug.Log(sb.ToString());
#endif
    }

	private int GetTournamentBestIndex(List<int> pickedCarIdList)
	{
		int bestIndex = int.MinValue;
		double highestFitness = double.MinValue;
		foreach (var pickedCarId in pickedCarIdList)
		{
			double currentFitness = 0;
			foreach (var stat in FitnessRecords)
			{
				if (stat.Id == pickedCarId)
				{
					currentFitness = stat.Fitness;
				}
			}
			if (currentFitness > highestFitness)
			{
				highestFitness = currentFitness;
				bestIndex = pickedCarId;
			}
		}
		return bestIndex;
	}

	protected override void RecombineAndMutate()
	{
        float mutationRateMinimum = (100 - MutationRate) / 100;
		float mutationRateMaximum = (100 + MutationRate) / 100;
		m_Top80Percent = (int)(PopulationSize * 0.8f);

		for (int i = 0; i < SavedCarNetworks.Length; i++)    // melyik autó
		{
			for (int j = 0; j < SavedCarNetworks[i].Length; j++) // melyik neuronréteg
			{
				for (int k = 0; k < SavedCarNetworks[i][j].Length; k++) // melyik neuron
				{
					for (int l = 0; l < SavedCarNetworks[i][j][k].Length; l++) // melyik súlya
					{
						if (i == FitnessRecords[0].Id)
						{
							CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
								SavedCarNetworks[i][j][k][l];
						}
						else if (i >= m_Top80Percent)
						{
							// Az autók 20%-a újra lesz randomolva minden körben.
							CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] = RandomHelper.NextFloat(-1f, 1f);
						}
						else
						{
							var mutation = RandomHelper.NextFloat(mutationRateMinimum, mutationRateMaximum);
							// 50% eséllyel örököl az egyik szülőtől.
							// carPairs[i] a két szülő indexét tartalmazza
							var index = CarPairs[i][RandomHelper.NextInt(0, 1)];

							// A MutationChance értékétől függően változik a mutáció valószínűsége
							if (RandomHelper.NextInt(1, 100) <= MutationChance)
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
