using UnityEngine;

[System.Serializable]
public abstract class NeuronLayer
{
    // public double[][] NeuronWeights { get; set; }
    public float[][] NeuronWeights { get; set; }

    protected int neuronCount;
    protected int inputCount;
    // protected double bias;
    protected float bias;

    // protected double weight;
    protected float weight;

    /// <summary>
    /// Creates a neuron layer.
    /// </summary>
    /// <param name="neuronCount"> The number of neurons in the layer.</param>
    /// <param name="inputCount"> The number of inputs the neuron layer gets (without the bias).</param>
    /// <param name="bias"> The bias value for the layer.</param>
    // public NeuronLayer(int neuronCount, int inputCount, double bias)
    public NeuronLayer(int neuronCount, int inputCount, float bias)
    {
        this.neuronCount = neuronCount;
        this.inputCount = inputCount;
        this.bias = bias;

        // NeuronWeights = new double[neuronCount][];
        NeuronWeights = new float[neuronCount][];
        for (int i = 0; i < NeuronWeights.Length; i++)
        {
            // +1 input is the bias
            // NeuronWeights[i] = new double[inputCount + 1];
            NeuronWeights[i] = new float[inputCount + 1];
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
                // TODO: modified
                // NeuronWeights[i][j] = Random.Range(-1f, 1f);
                NeuronWeights[i][j] = RandomHelper.NextFloat(-1f, 1f);
            }
        }
    }


    /// <summary>
    /// Calculates the output of the neuron layer.
    /// </summary>
    /// <param name="inputs"> An array which contains the output of the previous layer.</param>
    /// <returns> Returns the output array of the neuron layer.</returns>
    // public double[] CalculateLayer(double[] inputs)
    public float[] CalculateLayer(float[] inputs)
    {
        // double[] layerOutput = new double[neuronCount];
        float[] layerOutput = new float[neuronCount];

        // for each neuron -
        for (int i = 0; i < neuronCount; i++)
        {
            // double weightedSum = 0;
            float weightedSum = 0;
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


    // protected abstract double Activate(double x);
    protected abstract float Activate(float x);

}
