public class GeneticAlgorithmRouletteWheel : GeneticAlgorithm
{
    private struct WheelItem
    {
        public int Id;
        public float NormalizedFitness;
        public float LowerBound;
        public float UpperBound;
    }

    private WheelItem[] m_WheelItems;

    private void Start()
    {
        CarNetworks = new NeuralNetwork[PopulationSize];

        CarPairs = new int[PopulationSize][];
        for (int i = 0; i < CarPairs.Length; i++)
        {
            CarPairs[i] = new int[2];
        }

        FitnessRecords = new FitnessRecord[PopulationSize];
        for (int i = 0; i < FitnessRecords.Length; i++)
        {
            FitnessRecords[i] = new FitnessRecord();
        }

        m_WheelItems = new WheelItem[PopulationSize];
    }

    protected override void Selection()
    {
        float minFitness = float.MaxValue;
        float maxFitness = float.MinValue;

        // Searches for the minimum and maximum fitnesses
        foreach (var stat in FitnessRecords)
        {
            if (stat.Fitness < minFitness)
                minFitness = stat.Fitness;
            if (stat.Fitness > maxFitness)
                maxFitness = stat.Fitness;
        }

        float sum = 0;

        // Normalizes the fitness values
        for (int i = FitnessRecords.Length - 1; i >= 0; i--)
        {
            m_WheelItems[i].Id = FitnessRecords[i].Id;
            m_WheelItems[i].NormalizedFitness = FitnessRecords[i].Fitness - minFitness;
            sum += m_WheelItems[i].NormalizedFitness;
        }

        float wheelUnit = 1.0f / sum;
        float current = 0;

        // Calculates the lower and upper bounds
        for (int i = 0; i < m_WheelItems.Length; i++)
        {
            m_WheelItems[i].LowerBound = current;
            m_WheelItems[i].NormalizedFitness = m_WheelItems[i].NormalizedFitness * wheelUnit;
            current += m_WheelItems[i].NormalizedFitness;
            m_WheelItems[i].UpperBound = current;
        }

        // Selects the parents from the "wheel"
        foreach (int[] carPair in CarPairs)
        {
            float leftParent = RandomHelper.NextFloat(0, 1);
            float rightParent = RandomHelper.NextFloat(0, 1);

            // Selects the left parent
            for (int j = 0; j < m_WheelItems.Length; j++)
            {
                if (m_WheelItems[j].LowerBound <= leftParent && m_WheelItems[j].UpperBound >= leftParent)
                {
                    carPair[0] = m_WheelItems[j].Id;
                    break;
                }
            }
            // Selects the right parent
            for (int j = 0; j < m_WheelItems.Length; j++)
            {
                if (m_WheelItems[j].LowerBound <= rightParent && m_WheelItems[j].UpperBound >= rightParent)
                {
                    carPair[0] = m_WheelItems[j].Id;
                    break;
                }
            }
        }
        // The parents are selected
    }
}

