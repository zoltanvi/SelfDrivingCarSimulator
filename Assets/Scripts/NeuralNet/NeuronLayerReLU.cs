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


