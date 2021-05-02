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

[System.Serializable]
public class GameSave
{
    #region Manager értékei
    public int SelectionMethod;
    public int MutationChance;
    public float MutationRate;
    public int CarCount;
    public int LayersCount;
    public int NeuronPerLayerCount;
    public bool Navigator;
    public int TrackNumber;
    #endregion

    #region GeneticAlgorithm értékei
    // Az összes autó neurális hálójának értékeit tárolja
    public float[][][][] SavedCarNetworks;
    public int GenerationCount;
    #endregion

    #region Statisztikai adatok
    public List<float> MaxFitness;
    public List<float> MedianFitness;
    #endregion

    #region Egyéb adatok
    public int Seed;
    #endregion
}

