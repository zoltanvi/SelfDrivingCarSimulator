using System;

[System.Serializable]
public abstract class NeuronLayer
{
	public double[][] NeuronWeights { get; set; }

	protected static Random rand = new Random();
	protected int neuronCount;
	protected int inputCount;
	protected double bias;

	protected double weight;

	/// <summary>
	/// Creates a neuron layer.
	/// </summary>
	/// <param name="neuronCount"> The number of neurons in the layer.</param>
	/// <param name="inputCount"> The number of inputs the neuron layer gets (without the bias).</param>
	/// <param name="bias"> The bias value for the layer.</param>
	public NeuronLayer(int neuronCount, int inputCount, double bias)
	{
		this.neuronCount = neuronCount;
		this.inputCount = inputCount;
		this.bias = bias;

		NeuronWeights = new double[neuronCount][];
		for (int i = 0; i < NeuronWeights.Length; i++)
		{
			// +1 input is the bias
			NeuronWeights[i] = new double[inputCount + 1];
		}

		InitWeights();
	}

	/// <summary>
	/// Initializes the weights in the layer for all neurons to a random number
	/// between -1 and 1.
	/// </summary>
	protected void InitWeights()
	{
		for (int i = 0; i < NeuronWeights.Length; i++)
		{
			for (int j = 0; j < NeuronWeights[i].Length; j++)
			{
				// Get a random number between -1 and 1
				NeuronWeights[i][j] = (double)(rand.Next(-10, 11)) / 10;
			}
		}
	}


	/// <summary>
	/// Calculates the output of the neuron layer.
	/// </summary>
	/// <param name="inputs"> An array which contains the output of the previous layer.</param>
	/// <returns> Returns the output array of the neuron layer.</returns>
	public double[] CalculateLayer(double[] inputs)
	{
		double[] layerOutput = new double[neuronCount];

		// for each neuron -
		for (int i = 0; i < neuronCount; i++)
		{
			double weightedSum = 0;
			// - calculate the output
			for (int j = 0; j < inputCount; j++)
			{
				weightedSum += inputs[j] * NeuronWeights[i][j];
			}
			weightedSum += bias * NeuronWeights[i][inputCount];

			layerOutput[i] = Activate(weightedSum);
		}
		return layerOutput;
	}


	protected abstract double Activate(double x);

}
