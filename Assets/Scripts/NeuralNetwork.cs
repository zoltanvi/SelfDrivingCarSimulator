using UnityEngine;
using System;

public class NeuralNetwork : MonoBehaviour
{

	private int carIndex;
	public NeuronLayer[] NeuronLayers { get; set; }
	private int hiddenLayerCount;
	// Neuronok szama retegenkent.
	private int neuronCount;
	private int inputCount;
	private int transferCount;
	private double[][] transferData;
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
		transferCount = CarGameManager.Instance.HiddenLayerCount + 1;
		neuronCount = CarGameManager.Instance.NeuronPerLayerCount;
		inputCount = CarGameManager.Instance.CarsRayCount + 1;

		transferData = new double[transferCount][];
		for (int i = 0; i < transferData.Length; i++)
		{
			transferData[i] = new double[neuronCount];
		}

		carInputs = CarGameManager.Instance.Cars[carIndex].Inputs;

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
			+ "\n" + (carIndex + 1) + ". car:\n";
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
		CarGameManager.Instance.Cars[carIndex].NeuralNetworkText = carNNWeights;
		#endregion


	}


	void FixedUpdate()
	{
		carInputs = CarGameManager.Instance.Cars[carIndex].Inputs;

		// Az input reteg az auto tavolsagadatait + sebesseget kapja meg
		transferData[0] = NeuronLayers[0].CalculateLayer(carInputs);

		// A tobbi reteg az elozo reteg adatait kapja meg
		// TODO: közvetlenul is tovább lehet adni az adatokat
		for (int i = 1; i < transferData.Length; i++)
		{
			transferData[i] = NeuronLayers[i].CalculateLayer(transferData[i - 1]);
		}
		// Az output reteg az iranyitasra van kotve
		control = NeuronLayers[NeuronLayers.Length - 1].CalculateLayer(transferData[transferData.Length - 1]);

		carController.steer = control[0];
		carController.accelerate = control[1];

	}

}
