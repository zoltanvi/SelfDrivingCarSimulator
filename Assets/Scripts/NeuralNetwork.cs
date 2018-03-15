using UnityEngine;
using System;

public class NeuralNetwork : MonoBehaviour
{

	private int carIndex;
	public NeuronLayer[] NeuronLayers { get; set; }
	// Az input réteg (0.), és az output réteg (utolsó), nem számít bele!
	private int hiddenLayerCount;
	// Neuronok száma rétegenként (az input réteg igen, az output réteg nem számít bele!)
	private int neuronCount;
	private int inputCount;
	private int koztesAdatDarab;
	private double[][] koztesAdatok;
	private double[] carInputs;
	private double bias;

	public CarController carController;

	// Az auto iranyitasa a tombelemek alapjan tortenik
	// control[0] = kanyarodas , control[1] = gyorsulas 
	double[] control = new double[2];


	void Start()
	{
		bias = CarGameManager.Instance.Bias;
		carIndex = this.gameObject.GetComponent<CarController>().carStats.index;
		hiddenLayerCount = CarGameManager.Instance.HiddenLayerCount;
		koztesAdatDarab = CarGameManager.Instance.HiddenLayerCount + 1;
		neuronCount = CarGameManager.Instance.NeuronPerLayerCount;
		inputCount = CarGameManager.Instance.CarsRayCount + 1;

		koztesAdatok = new double[koztesAdatDarab][];
		for (int i = 0; i < koztesAdatok.Length; i++)
		{
			koztesAdatok[i] = new double[neuronCount];
		}

		carInputs = CarGameManager.Instance.AllCarInputs[carIndex];

		NeuronLayers = new NeuronLayer[hiddenLayerCount + 2];


		// Initialize the input layer and the hidden layers
		NeuronLayers[0] = new NeuronLayer(neuronCount, inputCount, bias);
		for (int i = 1; i < NeuronLayers.Length - 1; i++)
		{
			NeuronLayers[i] = new NeuronLayer(neuronCount, neuronCount, bias);
		}
		// Initialize the output layer
		NeuronLayers[hiddenLayerCount + 1] = new NeuronLayer(2, neuronCount, bias);


		#region Neuronhalo print

		DateTime localDate = DateTime.Now;
		string carNNWeights = localDate.ToString() 
			+ "\n" + (carIndex+1) + ". car:\n";
		for (int i = 0; i < NeuronLayers.Length; i++)
		{
			carNNWeights += (i + 1) + ". layer: \n";
			for (int k = 0; k < NeuronLayers[i].NeuronWeights.Length; k++)
			{
				for (int j = 0; j < NeuronLayers[i].NeuronWeights[0].Length; j++)
				{
					string tmp = string.Format("{0,10}", NeuronLayers[i].NeuronWeights[k][j]);
					carNNWeights += tmp + "\t";
				}
				carNNWeights += "\n";
			}
			carNNWeights += "\n";
		}
		CarGameManager.Instance.carNNWeights[carIndex] = carNNWeights;
		//Debug.Log(kk);
		#endregion

	}


	void FixedUpdate()
	{
		carInputs = CarGameManager.Instance.AllCarInputs[carIndex];

		// Az input réteg az autó szenzorait kapja inputként
		koztesAdatok[0] = NeuronLayers[0].CalculateLayer(carInputs);
		for (int i = 1; i < koztesAdatok.Length; i++)
		{
			koztesAdatok[i] = NeuronLayers[i].CalculateLayer(koztesAdatok[i - 1]);
		}

		control = NeuronLayers[NeuronLayers.Length - 1].CalculateLayer(koztesAdatok[koztesAdatok.Length - 1]);

		carController.steer = control[0];
		carController.accelerate = control[1];

	}

}
