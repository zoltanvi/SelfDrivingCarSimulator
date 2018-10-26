using System;


[System.Serializable]
internal class NeuronLayerTanh : NeuronLayer
{
	/// <summary>
	/// Creates a neuron layer.
	/// </summary>
	/// <param name="neuronCount"> The number of neurons in the layer.</param>
	/// <param name="inputCount"> The number of inputs the neuron layer gets (without the bias).</param>
	/// <param name="bias"> The bias value for the layer.</param>
	public NeuronLayerTanh(int neuronCount, int inputCount, float bias) : base(neuronCount, inputCount, bias) { }

	/// <summary>
	/// The activation function which is tahn(x) for us now.
	/// </summary>
	/// <param name="x"> The function parameter.</param>
	/// <returns>Returns the calculated function value which is between -1 and 1.</returns>
	// protected override double Activate(double x)
	protected override float Activate(float x)
	{
		// tanh(x) function
		float e2x = (float)Math.Exp(2 * x);
		return (e2x - 1) / (e2x + 1);
	}
}


