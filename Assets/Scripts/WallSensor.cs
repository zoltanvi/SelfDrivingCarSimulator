using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WallSensor : MonoBehaviour
{
	[Header("The origin point of the rays")]
	[SerializeField]
	private Transform rayOriginPoint;
	[SerializeField] [Range(0f, 10f)] private float lineLength = 5f;
	[SerializeField] [Range(1, 10)] private int numberOfRays = 3;
	[SerializeField] private string rayLayerName = "Environment";
	[SerializeField] private FitnessMeter fitnessMeter;
	[SerializeField] private Rigidbody carRigidbody;
	[SerializeField] private CarController carController;

	//[SerializeField] private GameObject carManager;
	//private CarManager myCarManager;

	private string rawSensorText = "";
	private GameObject[] rayHolders;
	private NeuronLayer neuronLayer1, neuronLayer2;
	private double[] tempNeuronData;
	//private double[] weights1 = new double[] { -0.4, 0.1, 0.6, 0 };
	//private double[] weights2 = new double[] { -0.4, 0.8, 0, 0 };

	double[] control = new double[2];

	private int carIndex;

	// A perceptron inputjai.
	private double[] raysAndFitness;
	// A raycastHit-ben vannak a sugarak adatai tarolva.
	private RaycastHit[] raycastHit;


	void Start()
	{
		//carIndex = CarManager.carRIndex++;
		carIndex = CarGameManager.Instance.carIndexD++;

		neuronLayer1 = new NeuronLayer(4, numberOfRays + 1);
		tempNeuronData = new double[4];
		neuronLayer2 = new NeuronLayer(2, 4);

		raysAndFitness = new double[numberOfRays + 1];

		raycastHit = new RaycastHit[numberOfRays];
		rayHolders = new GameObject[numberOfRays];

		InitializeLines();

		//Debug.Log(neuronLayer1.msg);
		//Debug.Log(neuronLayer2.msg);
	}

	void FixedUpdate()
	{
		CreateRays(numberOfRays);

		#region perceptron es fitness

		rawSensorText = "";
		for (int i = 0; i < raysAndFitness.Length - 1; i++)
		{
			if (null != raycastHit[i].collider)
			{
				raysAndFitness[i] = raycastHit[i].distance;
			}
			else
			{
				raysAndFitness[i] = lineLength;
			}

			// Szenzor adatok formazasa.
			rawSensorText += (i + 1) + ". sensor: " +
				string.Format("{0:0.0000}", raysAndFitness[i]) + "\n";
		}

		//CarManager.carDistances[carIndex] = rawSensorText;
		CarGameManager.Instance.carDistances[carIndex] = rawSensorText;

		raysAndFitness[raysAndFitness.Length - 1] = carRigidbody.velocity.magnitude;


		tempNeuronData = neuronLayer1.CalculateLayer(raysAndFitness);
		control = neuronLayer2.CalculateLayer(tempNeuronData);
		//Debug.Log(control[0] + " : kanyarodas,  " + control[1] + " : gyorsulas");
		carController.steer = control[0];
		carController.accelerate = control[1];


		#endregion



		//Debug.Log(this.transform.name + "\'s perceptron : " + perceptron.CalculateOutput(raysAndFitness));
	}

	// Letrehozza az erzekelo sugarakat.
	void CreateRays(int quantity)
	{
		// Az angleBase = a sugarak kozotti szog nagysaga.
		float angleBase = 180f / (quantity + 1);

		if (quantity == 5)
		{
			for (int i = 0; i < quantity; i++)
			{
				angleBase = 180f / (quantity - 1);
				// A jelenlegi sugar szoge balrol jobbra szamitva.
				Quaternion lineRotation =
					Quaternion.AngleAxis((45 * i), (rayOriginPoint.up));

				// A sugar kezdo es vegpontjai.
				Vector3 rayOrigin = rayOriginPoint.position;
				Vector3 rayDirection = lineRotation * (-rayOriginPoint.right);

				Physics.Raycast(
					rayOrigin,
					rayDirection,
					out raycastHit[i],
					lineLength,
					LayerMask.GetMask(rayLayerName));

				// Megrajzolja a sugarat. 
				rayHolders[i].GetComponent<LineRenderer>().SetPosition(0, rayOrigin);
				rayHolders[i].GetComponent<LineRenderer>().SetPosition(1, rayOrigin + rayDirection * lineLength);

			}
		}
		else
		{
			for (int i = 1; i < (quantity + 1); i++)
			{
				// A jelenlegi sugar szoge balrol jobbra szamitva.
				Quaternion lineRotation =
					Quaternion.AngleAxis((angleBase * i), (rayOriginPoint.up));

				// A sugar kezdo es vegpontjai.
				Vector3 rayOrigin = rayOriginPoint.position;
				Vector3 rayDirection = lineRotation * (-rayOriginPoint.right);

				// Letrehozza a sugarat es eltarolja hogy hozzaernek-e a falhoz.
				Physics.Raycast(
					rayOrigin,
					rayDirection,
					out raycastHit[i - 1],
					lineLength,
					LayerMask.GetMask(rayLayerName));

				// Megrajzolja a sugarat. 
				rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(0, rayOrigin);
				rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(1, rayOrigin + rayDirection * lineLength);
			}
		}


	}

	// Inicializalja a sugarakat reprezentalo vonalakat.
	private void InitializeLines()
	{
		Material lineMat = new Material(Shader.Find("Sprites/Default"));

		for (int i = 0; i < rayHolders.Length; i++)
		{
			rayHolders[i] = new GameObject();
			rayHolders[i].AddComponent<LineRenderer>();
			rayHolders[i].GetComponent<LineRenderer>().positionCount = 2;
			rayHolders[i].GetComponent<LineRenderer>().numCapVertices = 5;
			rayHolders[i].GetComponent<LineRenderer>().startWidth = 0.08f;
			rayHolders[i].GetComponent<LineRenderer>().endWidth = 0.08f;
			rayHolders[i].GetComponent<LineRenderer>().useWorldSpace = false;
			rayHolders[i].GetComponent<LineRenderer>().material = lineMat;
			rayHolders[i].GetComponent<LineRenderer>().startColor = Color.blue;
			rayHolders[i].GetComponent<LineRenderer>().endColor = Color.cyan;

		}
	}

}
