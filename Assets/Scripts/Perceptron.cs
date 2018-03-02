using System;

public class Perceptron
{

	private double[] weights;

	// n az inputok szama.
	public Perceptron(int n)
	{
		weights = new double[n];
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = 0.1d;
		}
	}


	public double CalculateOutput(double[] inputs)
	{
		double sum = 0d;
		for (int i = 0; i < weights.Length; i++)
		{
			sum += inputs[i] * weights[i];
		}

		return Sigmoid(sum);
	}


	private double Sigmoid(double x)
	{
		return 1d / (1d + Math.Exp(-x));
	}


}
