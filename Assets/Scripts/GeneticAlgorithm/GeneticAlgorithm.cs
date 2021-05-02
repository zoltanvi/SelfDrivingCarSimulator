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

using System;
using UnityEngine;

public abstract class GeneticAlgorithm : MonoBehaviour
{
    protected struct FitnessRecord : IComparable
    {
        public int Id;
        public float Fitness;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;  // this is greater than obj

            if (!(obj is FitnessRecord)) throw new ArgumentException("Object is not a FitnessRecord!");
            FitnessRecord other = (FitnessRecord)obj;
            return Fitness.CompareTo(other.Fitness);
        }
    }

    public int PopulationSize { get; set; }

    protected FitnessRecord[] FitnessRecords;

    public float MutationChance { get; set; }
    public float MutationRate { get; set; }

    public int GenerationCount;
    protected NeuralNetwork[] CarNetworks;

    // Ebben a tömbben tárolja a szelekciókor létrejövő párokat.
    // Az első index a létrejövő pár sorszámát jelöli (ahány új autó kell)
    // A második index a bal (0) és jobb (1) szülőket jelöli.
    protected int[][] CarPairs;

    // 4D tömbben tárolja az összes autó neurális hálójának értékeit,
    // mert rekombinációkor az eredeti értékekkel kell dolgozni.
    public float[][][][] SavedCarNetworks;

    private Manager m_Manager;

    private void Awake()
    {
        m_Manager = Master.Instance.Manager;
        PopulationSize = m_Manager.Configuration.CarCount;
        MutationChance = m_Manager.Configuration.MutationChance; // 30-70 int %
        MutationRate = m_Manager.Configuration.MutationRate;   // 2-4 float %
    }

    private void Start()
    {
        CarNetworks = new NeuralNetwork[PopulationSize];
        CarPairs = new int[PopulationSize][];
        FitnessRecords = new FitnessRecord[PopulationSize];

        for (int i = 0; i < CarPairs.Length; i++)
        {
            CarPairs[i] = new int[2];
        }

        for (int i = 0; i < FitnessRecords.Length; i++)
        {
            FitnessRecords[i] = new FitnessRecord();
        }
    }

    private void FixedUpdate()
    {
        // Ha minden autó megfagyott, jöhet az új generáció
        if (m_Manager.AliveCount > 0) return;

        // Elmenti az összes autó neurális hálóját
        SaveNeuralNetworks();

        if (m_Manager.IsLoad)
        {
            GenerationCount = m_Manager.Save.GenerationCount;
            m_Manager.IsLoad = false;
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

        // Ha leállási feltétel meg volt adva és teljesült, leáll a szimuláció
        if (m_Manager.Configuration.StopConditionActive &&
            m_Manager.Configuration.StopGenerationNumber < GenerationCount)
        {
            Master.Instance.OnSimulationFinished();
        }
    }

    protected void RespawnCars()
    {
        // Respawnolja az összes autót 
        for (int i = 0; i < PopulationSize; i++)
        {
            m_Manager.Cars[i].PrevFitness = 0;
            m_Manager.Cars[i].IsAlive = true;
            m_Manager.SpawnFromPool(m_Manager.transform.position, m_Manager.transform.rotation);
        }

        // Ha a player játszik, a piros autót is respawnolja
        if (m_Manager.ManualControl)
        {
            m_Manager.SpawnPlayerCar(m_Manager.transform.position, m_Manager.transform.rotation);
        }

        m_Manager.SetBackTimes();
        // Növeli a generáció számlálót
        GenerationCount++;
        m_Manager.UiPrinter.GenerationCount = GenerationCount;
    }

    /// <summary>
    /// Inicilizálja a savedCarNetwork tömböt, melyben a neurális hálók vannak tárolva.
    /// </summary>
    public void InitSavedCarNetwork()
    {
        for (int i = 0; i < PopulationSize; i++)
        {
            CarNetworks[i] = m_Manager.Cars[i].NeuralNetwork;
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
            FitnessRecords[i].Id = m_Manager.Cars[i].Id;
            FitnessRecords[i].Fitness = m_Manager.Cars[i].Fitness;
        }

        //  ... majd rendezi a stats tömböt csökkenő sorrendbe
        Array.Sort(FitnessRecords);
        Array.Reverse(FitnessRecords);
    }

    protected abstract void Selection();

    protected virtual void RecombineAndMutate()
    {
        float mutationRateMinimum = (100 - MutationRate) / 100;
        float mutationRateMaximum = (100 + MutationRate) / 100;

        for (int i = 0; i < SavedCarNetworks.Length; i++)    // which car
        {
            for (int j = 0; j < SavedCarNetworks[i].Length; j++) // which neuron layer
            {
                for (int k = 0; k < SavedCarNetworks[i][j].Length; k++) // which neuron
                {
                    for (int l = 0; l < SavedCarNetworks[i][j][k].Length; l++) // which weight
                    {
                        if (i == FitnessRecords[0].Id)
                        {
                            CarNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
                                SavedCarNetworks[i][j][k][l];
                        }
                        else
                        {
                            float mutation = RandomHelper.NextFloat(mutationRateMinimum, mutationRateMaximum);
                            
                            // 50% eséllyel örököl az egyik szülőtől.
                            // carPairs[i] a két szülő indexét tartalmazza
                            int index = CarPairs[i][RandomHelper.NextInt(0, 1)];

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
            if (max < m_Manager.Cars[i].Fitness)
            {
                max = m_Manager.Cars[i].Fitness;
            }
        }

        // The FitnessRecords is sorted so we know which car is the median
        float median = m_Manager.Cars[FitnessRecords[PopulationSize / 2].Id].Fitness;

        m_Manager.MaxFitness.Add(max);
        m_Manager.MedianFitness.Add(median);
    }
}

