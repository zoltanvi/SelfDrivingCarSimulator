using System;
using System.Threading;


public class NeuronLayer
{
	private Neuron[] neurons;
	private double[] neuronInputs;
	public string msg;

	/// <summary>
	/// Letrehoz egy reteget neuronokbol.
	/// </summary>
	/// <param name="n"> A neuronok szama a retegben.</param>
	/// <param name="inputNumber"> Az inputok szama a retegben.</param>
	public NeuronLayer(int n, int inputNumber)
	{
		neurons = new Neuron[n];
		for (int i = 0; i < neurons.Length; i++)
		{
			neurons[i] = new Neuron(inputNumber);
			msg = neurons[i].msg + "\n";
		}

	}

	/// <summary>
	/// Letrehoz egy reteget neuronokból.
	/// Megadhatoak az elek sulyai.
	/// </summary>
	/// <param name="n"> A neuronok szama a retegben.</param>
	/// <param name="inputNumber"> Az inputok szama a retegben.</param>
	/// <param name="_weights"> Az elek sulyai a retegben.</param>
	public NeuronLayer(int n, int inputNumber, double[] _weights)
	{
		neurons = new Neuron[n];
		for (int i = 0; i < neurons.Length; i++)
		{
			neurons[i] = new Neuron(inputNumber, _weights);
			//msg = neurons[i].msg + "\n";
		}

	}


	/// <summary>
	/// Kiszamitja a reteg neuronjainak outputjat.
	/// </summary>
	/// <returns> Visszater a reteg neuronjainak outputjaval.</returns>
	public double[] CalculateLayer(double[] inputs)
	{
		double[] layerOutput = new double[neurons.Length];

		for (int i = 0; i < neurons.Length; i++)
		{
			layerOutput[i] = neurons[i].Output(inputs);
		}

		return layerOutput;
	}


}



public class Neuron
{
	private double[] weights;
	public string msg;
	Random rand = new Random();

	/// <summary>
	/// Letrehoz egy neuront az inputok szama alapjan.
	/// </summary>
	/// <param name="n"> A neuron inputjainak szama.</param>
	public Neuron(int n)
	{
		weights = new double[n];

		for (int i = 0; i < weights.Length; i++)
		{
			Thread.Sleep(1);
			// 0 = negativ, 1 = pozitiv
			int r = rand.Next(0, 2);
			double tmp = rand.Next(0, 11);
			tmp /= 10;
			weights[i] = r < 1 ? tmp : tmp * (-1);
			msg += "  w" + i + ": " + weights[i];
		}
	}


	/// <summary>
	/// Letrehoz egy neuront.
	/// Megadhatoak a sulyok.
	/// </summary>
	/// <param name="n"> A neuron inputjainak a szama.</param>
	/// <param name="_weights"> A sulyok tombje.</param>
	public Neuron(int n, double[] _weights)
	{
		weights = new double[n];

		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = _weights[i];
			//msg += "  w" + i + ": " + weights[i];
		}
	}


	/// <summary>
	/// General egy outputot az aktivacios fugvennyel.
	/// </summary>
	/// <param name="inputs"> A neuron inputjainak tombje.</param>
	/// <returns> Visszater egy neuron outputjaval.</returns>
	public double Output(double[] inputs)
	{
		double sum = 0d;
		for (int i = 0; i < weights.Length; i++)
		{
			sum += inputs[i] * weights[i];
		}

		return Activate(sum);
	}


	private double Activate(double x)
	{
		return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
	}

}

