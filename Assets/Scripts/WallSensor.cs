using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WallSensor : MonoBehaviour
{
	[Header("The origin point of the rays")]
	[SerializeField] private Transform rayOriginPoint;
	[SerializeField] [Range(0f, 10f)] private float lineLength = 5f;
	[SerializeField] [Range(1, 10)] private int numberOfRays = 3;
	[SerializeField] private string rayLayerName = "Environment";
	[SerializeField] private TextMeshProUGUI sensorText;
	[SerializeField] private FitnessMeter fitnessMeter;

	private string rawSensorText = "";
	private GameObject[] rayHolders;

	private Perceptron perceptron;
	// A perceptron inputjai.
	private double[] raysAndFitness;
	// A raycastHit-ben vannak a sugarak adatai tarolva.
	private RaycastHit[] raycastHit;


	void Start()
	{
		perceptron = new Perceptron(numberOfRays + 1);
		raysAndFitness = new double[numberOfRays + 1];

		raycastHit = new RaycastHit[numberOfRays];
		rayHolders = new GameObject[numberOfRays];

		InitializeLines();

	}

	void FixedUpdate()
	{
		CreateRays(numberOfRays);

		#region perceptron es fitness
		rawSensorText = "";
		for (int i = 0; i < raysAndFitness.Length - 1; i++)
		{

			raysAndFitness[i] = raycastHit[i].distance;

			// Szenzor adatok kiiratasa.
			rawSensorText += (i + 1) + ". sensor: " +
				string.Format("{0:0.0000}", raysAndFitness[i]) + "\n";
		}
		sensorText.text = rawSensorText;

		// A perceptron utolso inputja az auto fitnesse.
		raysAndFitness[raysAndFitness.Length - 1] = fitnessMeter.absoluteFitness;

		#endregion

		Debug.Log(this.transform.name + "\'s perceptron : " + perceptron.CalculateOutput(raysAndFitness));
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
