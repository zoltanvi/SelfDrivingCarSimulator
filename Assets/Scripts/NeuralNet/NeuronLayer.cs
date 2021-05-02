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

[System.Serializable]
public abstract class NeuronLayer
{
    public float[][] NeuronWeights { get; set; }

    protected int NeuronCount;
    protected int InputCount;
    protected float Bias;

    /// <summary>
    /// Creates a neuron layer.
    /// </summary>
    /// <param name="neuronCount"> The number of neurons in the layer.</param>
    /// <param name="inputCount"> The number of inputs the neuron layer gets (without the bias).</param>
    /// <param name="bias"> The bias value for the layer.</param>
    protected NeuronLayer(int neuronCount, int inputCount, float bias)
    {
        this.NeuronCount = neuronCount;
        this.InputCount = inputCount;
        this.Bias = bias;

        NeuronWeights = new float[neuronCount][];
        for (int i = 0; i < NeuronWeights.Length; i++)
        {
            // +1 input is the bias
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
        float[] layerOutput = new float[NeuronCount];

        // for each neuron -
        for (int i = 0; i < NeuronCount; i++)
        {
            float weightedSum = 0;
            // - calculate the output
            for (int j = 0; j < InputCount; j++)
            {
                weightedSum += inputs[j] * NeuronWeights[i][j];
            }
            weightedSum += Bias * NeuronWeights[i][InputCount];

            layerOutput[i] = Activate(weightedSum);
        }
        return layerOutput;
    }

    protected abstract float Activate(float x);
}

