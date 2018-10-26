using UnityEngine;


[System.Serializable]
public class NeuralNetwork : MonoBehaviour
{

	public NeuronLayer[] NeuronLayers { get; set; }

	private int carID;
	private int hiddenLayerCount;
	private int neuronCount;
	private int inputCount;
	private float[][] transferData;
	private float[] carInputs;
	private float bias;

	public CarController CarController;

	// Az auto iranyitasa a tombelemek alapjan tortenik
	// control[0] = kanyarodas , control[1] = gyorsulas 
	float[] control = new float[2];

	private void Start()
	{
		bias = Master.Instance.Manager.Bias;
		carID = this.gameObject.GetComponent<CarController>().Id;
		hiddenLayerCount = Master.Instance.Manager.LayersCount;
		neuronCount = Master.Instance.Manager.NeuronPerLayerCount;

		if (Master.Instance.Manager.Navigator)
		{
			inputCount = Master.Instance.Manager.CarSensorCount + 4;
		}
		else
		{
			inputCount = Master.Instance.Manager.CarSensorCount + 1;
		}


		transferData = new float[hiddenLayerCount][];
		for (int i = 0; i < transferData.Length; i++)
		{
			transferData[i] = new float[neuronCount];
		}

		// hidden layers, +1 output layer
		NeuronLayers = new NeuronLayer[hiddenLayerCount + 1];

		switch (hiddenLayerCount)
		{
			// Initialize the hidden layers.
			// If zero hidden layer -> there is only the output layer
			case 0:
				NeuronLayers[0] = new NeuronLayerTanh(2, inputCount, bias);
				break;

			// If one hidden layer -> first layer gets the input,
			// second layer is the output layer.
			case 1:
				NeuronLayers[0] = new NeuronLayerTanh(neuronCount, inputCount, bias);
				NeuronLayers[1] = new NeuronLayerTanh(2, neuronCount, bias);
				break;

			// If two or more hidden layers -> first layer gets the input,
			// the other ones get the output from the previous layer
			// and the last layer is the output layer.
			default:
				NeuronLayers[0] = new NeuronLayerTanh(neuronCount, inputCount, bias);
				for (int i = 1; i < NeuronLayers.Length - 1; i++)
				{
					NeuronLayers[i] = new NeuronLayerTanh(neuronCount, neuronCount, bias);
				}
				NeuronLayers[NeuronLayers.Length - 1] = new NeuronLayerTanh(2, neuronCount, bias);
				break;
		}

		if (!Master.Instance.Manager.WasItALoad) return;


		for (int i = 0; i < NeuronLayers.Length; i++)
		{
			for (int j = 0; j < NeuronLayers[i].NeuronWeights.Length; j++)
			{
				for (int k = 0; k < NeuronLayers[i].NeuronWeights[j].Length; k++)
				{
					NeuronLayers[i].NeuronWeights[j][k] = Master.Instance.Manager.Save.SavedCarNetworks[carID][i][j][k];
				}
			}
		}

	}


	private void FixedUpdate()
	{
		// The inputs array contains the car's sensor datas and it's current speed.
		carInputs = Master.Instance.Manager.Cars[carID].Inputs;

		switch (hiddenLayerCount)
		{
			// If zero hidden layer -> there is only the output layer
			case 0:
				control = NeuronLayers[0].CalculateLayer(carInputs);
				break;

			// If one hidden layer -> first layer gets the input,
			// second layer is the output layer.
			case 1:
				control = NeuronLayers[1].CalculateLayer(
					   NeuronLayers[0].CalculateLayer(carInputs));
				break;

			// If two or more hidden layers -> first layer gets the input,
			// the other ones get the output from the previous layer
			// and the last layer is the output layer.
			default:
				transferData[0] = NeuronLayers[0].CalculateLayer(carInputs);
				for (int i = 1; i < transferData.Length; i++)
				{
					transferData[i] = NeuronLayers[i].CalculateLayer(transferData[i - 1]);
				}
				control = NeuronLayers[NeuronLayers.Length - 1].CalculateLayer(transferData[transferData.Length - 1]);
				break;
		}

		CarController.Steer = control[0];
		CarController.Accelerate = control[1];

	}

}

