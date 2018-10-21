using UnityEngine;
using System;

public abstract class GeneticAlgorithm : MonoBehaviour
{
    protected struct Stat : IComparable
    {
        public int ID;
        // public double Fitness;
        public float Fitness;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;  // this is greater than obj

            if (obj is Stat)
            {
                Stat other = (Stat)obj;
                return this.Fitness.CompareTo(other.Fitness);
            }
            throw new ArgumentException("Object is not a Stat!");
        }
    }

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
    // public double[][][][] SavedCarNetworks;
    public float[][][][] SavedCarNetworks;


    private Manager manager;

    void Awake()
    {
        manager = Master.Instance.Manager;
        PopulationSize = manager.CarCount;
        MutationChance = manager.MutationChance; // 30-70 int %
        MutationRate = manager.MutationRate;   // 2-4 float %

    }

    void Start()
    {
        carNetworks = new NeuralNetwork[PopulationSize];

        carPairs = new int[PopulationSize][];
        int carPairsLength = carPairs.Length;
        for (int i = 0; i < carPairsLength; i++)
        {
            carPairs[i] = new int[2];
        }

        stats = new Stat[PopulationSize];
        int statsLength = stats.Length;
        for (int i = 0; i < statsLength; i++)
        {
            stats[i] = new Stat();
        }

    }

    void FixedUpdate()
    {

        // Ha minden autó megfagyott, jöhet az új generáció
        if (manager.AliveCount <= 0)
        {
            // Elmenti az összes autó neurális hálóját
            SaveNeuralNetworks();

            if (manager.wasItALoad)
            {
                GenerationCount = manager.Save.GenerationCount;
                manager.wasItALoad = false;
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
        manager.myUIPrinter.GenerationCount = GenerationCount;
    }

    /// <summary>
    /// Inicilizálja a savedCarNetwork tömböt, melyben a neurális hálók vannak tárolva.
    /// </summary>
    public void InitSavedCarNetwork()
    {
        for (int i = 0; i < PopulationSize; i++)
        {
            carNetworks[i] = manager.Cars[i].NeuralNetwork;
        }

        // SavedCarNetworks = new double[PopulationSize][][][];
        SavedCarNetworks = new float[PopulationSize][][][];

        for (int i = 0; i < SavedCarNetworks.Length; i++)
        {
            // SavedCarNetworks[i] = new double[carNetworks[i].NeuronLayers.Length][][];
            SavedCarNetworks[i] = new float[carNetworks[i].NeuronLayers.Length][][];
        }
        for (int i = 0; i < SavedCarNetworks.Length; i++)
        {
            for (int j = 0; j < SavedCarNetworks[i].Length; j++)
            {
                // SavedCarNetworks[i][j] = new double[carNetworks[i].NeuronLayers[j].NeuronWeights.Length][];
                SavedCarNetworks[i][j] = new float[carNetworks[i].NeuronLayers[j].NeuronWeights.Length][];
            }
        }
        for (int i = 0; i < SavedCarNetworks.Length; i++)
        {
            for (int j = 0; j < SavedCarNetworks[i].Length; j++)
            {
                for (int k = 0; k < SavedCarNetworks[i][j].Length; k++)
                {
                    // SavedCarNetworks[i][j][k] = new double[carNetworks[i].NeuronLayers[j].NeuronWeights[k].Length];
                    SavedCarNetworks[i][j][k] = new float[carNetworks[i].NeuronLayers[j].NeuronWeights[k].Length];
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
            stats[i].ID = manager.Cars[i].ID;
            stats[i].Fitness = manager.Cars[i].Fitness;
        }

        //  ... majd rendezi a stats tömböt csökkenő sorrendbe
        Array.Sort(stats);
        Array.Reverse(stats);

    }


    protected abstract void Selection();


    protected virtual void RecombineAndMutate()
    {
        int index;
        float mutation;
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
                        if (i == stats[0].ID)
                        {
                            carNetworks[i].NeuronLayers[j].NeuronWeights[k][l] =
                                SavedCarNetworks[i][j][k][l];
                        }
                        else
                        {
                            // TODO: modified
                            //mutation = UnityEngine.Random.Range(mutationRateMinimum, mutationRateMaximum);
                            mutation = RandomHelper.NextFloat(mutationRateMinimum, mutationRateMaximum);
                            // 50% eséllyel örököl az egyik szülőtől.
                            // carPairs[i] a két szülő indexét tartalmazza
                            // TODO: modified
                            // index = carPairs[i][UnityEngine.Random.Range(0, 2)];
                            index = carPairs[i][RandomHelper.NextInt(0, 1)];

                            // A MutationChance értékétől függően változik a mutáció valószínűsége
                            // TODO: modified
                            // if (UnityEngine.Random.Range(0, 101) <= MutationChance)
                            if (RandomHelper.NextInt(0, 100) <= MutationChance)
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
        // double max = double.MinValue;
        // double median = double.MinValue;

        float max = float.MinValue;
        float median = float.MinValue;


        for (int i = 0; i < PopulationSize; i++)
        {
            if (max < manager.Cars[i].Fitness)
            {
                max = manager.Cars[i].Fitness;
            }
        }

        // A stats tömb sorba lesz rendezve,
        // így onnan tudjuk, hogy melyik a középső autó
        median = manager.Cars[
            stats[PopulationSize / 2].ID].Fitness;


        manager.maxFitness.Add(max);
        manager.medianFitness.Add(median);

    }


}
