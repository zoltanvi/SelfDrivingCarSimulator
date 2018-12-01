

public class GeneticAlgorithmRouletteWheel : GeneticAlgorithm
{
	private struct WheelItem
	{
		public int Id;
		public float NormalizedFitness;
		public float LowerBound;
		public float UpperBound;
	}

	WheelItem[] wheelItems;

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
		
		wheelItems = new WheelItem[PopulationSize];
	}

	protected override void Selection()
	{
		float minFitness = float.MaxValue;
		float maxFitness = float.MinValue;

		// Searches for the minimum and maximum fitnesses
		foreach (var stat in Stats)
		{
			if (stat.Fitness < minFitness)
				minFitness = stat.Fitness;
			if (stat.Fitness > maxFitness)
				maxFitness = stat.Fitness;
		}

		float sum = 0;

		// Normalizes the fitness values
		for (int i = Stats.Length - 1; i >= 0; i--)
		{
			wheelItems[i].Id = Stats[i].Id;
			wheelItems[i].NormalizedFitness = Stats[i].Fitness - minFitness;
			sum += wheelItems[i].NormalizedFitness;
		}

		float wheelUnit = 1.0f / sum;
		float current = 0;

		// Calculates the lower and upper bounds
		for (int i = 0; i < wheelItems.Length; i++)
		{
			wheelItems[i].LowerBound = current;
			wheelItems[i].NormalizedFitness = wheelItems[i].NormalizedFitness * wheelUnit;
			current += wheelItems[i].NormalizedFitness;
			wheelItems[i].UpperBound = current;
		}

		// Selects the parents from the "wheel"
		for (int i = 0; i < carPairs.Length; i++)
		{
			float leftParent = RandomHelper.NextFloat(0, 1);
			float rightParent = RandomHelper.NextFloat(0, 1);

			// Selects the left parent
			for (int j = 0; j < wheelItems.Length; j++)
			{
				if(wheelItems[j].LowerBound <= leftParent && wheelItems[j].UpperBound >= leftParent)
				{
					carPairs[i][0] = wheelItems[j].Id;
					break;
				}
			}
			// Selects the right parent
			for (int j = 0; j < wheelItems.Length; j++)
			{
				if (wheelItems[j].LowerBound <= rightParent && wheelItems[j].UpperBound >= rightParent)
				{
					carPairs[i][0] = wheelItems[j].Id;
					break;
				}
			}
		}
		// The parents are selected
	}
}

