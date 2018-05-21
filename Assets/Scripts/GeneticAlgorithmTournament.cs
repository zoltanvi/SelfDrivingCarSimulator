using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithmTournament : GeneticAlgorithm {

	// A tournament során ennyi autó "versenyzik" egyszerre egymással
	private int selectionPressure = 3;
	
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

		string tmp = "";
		for (int i = 0; i < carPairs.Length; i++)
		{
			tmp += carPairs[i][0] + " :: " + carPairs[i][1] + "\n";
		}

		Debug.Log(tmp);

	}

	private int GetTournamentBestIndex(List<int> picked)
	{
		int bestIndex = int.MinValue;
		double highestFitness = double.MinValue;
		foreach (var thispicked in picked)
		{
			double currentFitness = 0;
			foreach (var stat in stats)
			{
				if (stat.ID == thispicked)
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




}
