using System;

public class NeuronLayer
{
	public double[][] NeuronWeights { get; set; }

	private static Random rand = new Random();
	private int neuronCount;
	private int weightCount;
	private double bias;

	/// <summary>
	///  Egy réteg neuront hoz létre.
	/// </summary>
	/// <param name="neuronPerLayer"> A neuronok száma rétegenként.</param>
	/// <param name="layerInputCount"> A rétegre kötni kívánt inputok száma.</param>
	/// <param name="biasValue"> A bias értéke.</param>
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


	/// <summary>
	/// Inicializálja a neuronok inputjainak súlyait random értékre.
	/// A random érték -1.0 és 1.0 között van
	/// </summary>
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


	/// <summary>
	/// Kiszámolja a réteg outputját.
	/// </summary>
	/// <param name="inputs"> Az inputok tömbje.</param>
	/// <returns> Az outputok tömbjével.</returns>
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
	/// Az aktivációs függvény, jelen esetben a tanh(x) fuggvény. (Hiperbolikus tangens)
	/// A függveny értékkészlete (-1.0, 1.0)
	/// </summary>
	/// <param name="x"> Mely ponton számítsa ki a függvényértéket.</param>
	/// <returns> A kiszámított függvényértékkel.</returns>
	private double Activate(double x)
	{
		return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
	}

}
