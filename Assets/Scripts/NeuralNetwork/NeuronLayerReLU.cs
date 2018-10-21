using System;

[System.Serializable]
internal class NeuronLayerReLU : NeuronLayer
{
    /// <summary>
    /// Creates a neuron layer.
    /// </summary>
    /// <param name="neuronCount"> The number of neurons in the layer.</param>
    /// <param name="inputCount"> The number of inputs the neuron layer gets (without the bias).</param>
    /// <param name="bias"> The bias value for the layer.</param>
    // public NeuronLayerReLU(int neuronCount, int inputCount, double bias) : base(neuronCount, inputCount, bias) { }
    public NeuronLayerReLU(int neuronCount, int inputCount, float bias) : base(neuronCount, inputCount, bias) { }

    /// <summary>
    /// The activation function which is ReLU(x) for us now.
    /// </summary>
    /// <param name="x"> The function parameter.</param>
    /// <returns>Returns the calculated function value which is between 0 and x.</returns>
    // protected override double Activate(double x)
    protected override float Activate(float x)
    {
        // The rectifier
        return Math.Max(0, x);
    }
}

