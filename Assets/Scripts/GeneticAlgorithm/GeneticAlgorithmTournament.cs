/*
Copyright (C) 2021 zoltanvi

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GeneticAlgorithmTournament : GeneticAlgorithm
{
    // A tournament során ennyi autó "versenyzik" egyszerre egymással
    private int m_SelectionPressure = 3;

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
        foreach (int pickedCarId in pickedCarIdList)
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
}
