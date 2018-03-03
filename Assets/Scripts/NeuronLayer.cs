using System;
using System.Threading;


public class NeuronLayer
{
	private Neuron[] neurons;
	private double[] neuronInputs;
	public string msg;

	/// <summary>
	/// Létrehoz egy réteget neuronokból.
	/// </summary>
	/// <param name="n"> a neuronok száma a rétegben.</param>
	/// <param name="inputNumber"> az inputok száma a rétegben.</param>
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
	/// Létrehoz egy réteget neuronokból.
	/// MEgadhatóak az élek súlyai.
	/// </summary>
	/// <param name="n"> a neuronok száma a rétegben.</param>
	/// <param name="inputNumber">az inputok száma a rétegben.</param>
	/// <param name="_weights">az élek súlyai a rétegben.</param>
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
	/// Kiszámítja a réteg neuronjainak outputját.
	/// </summary>
	/// <returns> a réteg neuronjainak outputjával.</returns>
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
	/// Létrehoz egy neuront az inputok száma alapján.
	/// </summary>
	/// <param name="n"> a neuron inputjainak száma.</param>
	public Neuron(int n)
	{
		weights = new double[n];

		for (int i = 0; i < weights.Length; i++)
		{
			Thread.Sleep(1);
			// 0 = negatív, 1 = pozitív
			int r = rand.Next(0, 2);
			double tmp = rand.Next(0, 11);
			tmp /= 10;
			weights[i] = r < 1 ? tmp : tmp * (-1);
			msg += "  w" + i + ": " + weights[i];
		}
	}


	/// <summary>
	/// Létrehoz egy neuront.
	/// Megadhatóak a súlyok.
	/// </summary>
	/// <param name="n"> a neuron inputjainak a száma.</param>
	/// <param name="_weights">a súlyok tömbje.</param>
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
	/// Generál egy outputot az aktivációs fügvénnyel.
	/// </summary>
	/// <param name="inputs"> a neuron inputjainak tömbje.</param>
	/// <returns> egy neuron outputjával.</returns>
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

