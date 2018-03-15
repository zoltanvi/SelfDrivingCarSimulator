using System;

public class NeuronLayer
{
	public double[][] NeuronWeights { get; set; }

	static Random rand = new Random();
	int neuronCount;
	int weightCount;
	double bias;


	public NeuronLayer(int neuronPerLayer, int layerInputCount, double biasValue)
	{
		neuronCount = neuronPerLayer;
		weightCount = layerInputCount;
		bias = biasValue;

		NeuronWeights = new double[neuronCount][];
		for (int i = 0; i < NeuronWeights.Length; i++)
		{
			// +1 input a bias!
			NeuronWeights[i] = new double[weightCount + 1];
		}
		InitWeights();
	}

	void InitWeights()
	{
		for (int i = 0; i < NeuronWeights.Length; i++)
		{
			for (int j = 0; j < NeuronWeights[0].Length; j++)
			{
				int r = rand.Next(0, 2);
				double tmp = rand.Next(0, 11);
				tmp /= 10;
				NeuronWeights[i][j] = r < 1 ? tmp : tmp * (-1);
			}
		}
	}

	// az inputs tömbnek a mérete ugyan annyinak 
	// kell legyen, mint a súlyok darabszáma!
	public double[] CalculateLayer(double[] inputs)
	{
		double[] layerOutput = new double[neuronCount];

		for (int i = 0; i < neuronCount; i++)
		{
			double weightedSum = 0;
			for (int j = 0; j < weightCount; j++)
			{
				weightedSum += inputs[j] * NeuronWeights[i][j];
			}
			weightedSum += bias * NeuronWeights[i][weightCount];

			layerOutput[i] = Activate(weightedSum);
		}
		return layerOutput;
	}


	/// <summary>
	/// Az aktivacios fuggveny, jelen esetben a tanh(x) fuggveny. (Hiperbolikus tangens)
	/// A fuggveny ertekkeszlete (-1.0, 1.0)
	/// </summary>
	/// <param name="x"> Mely ponton szamitsa ki a fuggvenyerteket.</param>
	/// <returns> Visszater a kiszamitott fuggvenyertekkel.</returns>
	private double Activate(double x)
	{
		return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
	}

}
