using UnityEngine;
using UnityEngine.UI;

public class WallSensor : MonoBehaviour
{

	[SerializeField] private Transform rayOriginPoint;
	[SerializeField] private float lineDistance = 5f;
	[SerializeField] private int rayQuantity = 3;
	[SerializeField] private string rayLayerName = "Environment";
	[SerializeField] private Text sensorText;
	[SerializeField] private FitnessMeter fitnessMeter;

	private string rawSensorText = "";
	private GameObject[] rayHolders;
	private Perceptron perceptron;
	private double[] raysAndFitness;

	// A raycastHit-ben vannak a sugarak adatai tarolva.
	private RaycastHit[] raycastHit;

	void Start()
	{
		perceptron = new Perceptron(rayQuantity + 1);
		raysAndFitness = new double[rayQuantity + 1];

		raycastHit = new RaycastHit[rayQuantity];
		rayHolders = new GameObject[rayQuantity];

		InitializeLines();

	}

	void FixedUpdate()
	{
		CreateRays(rayQuantity);

		#region -- perceptron proba + kiiratas --
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


		//for (int i = 0; i < raysAndFitness.Length; i++)
		//{
		//	Debug.Log("rAF[" + i + "] = " + raysAndFitness[i]);
		//}
		Debug.Log(this.transform.name +  " f : " + perceptron.FeedForward(raysAndFitness));
	}

	// Letrehozza az erzekelo sugarakat.
	void CreateRays(int quantity)
	{
		// Az angleBase = a sugarak kozotti szog nagysaga.
		float angleBase = 180f / (quantity + 1);
		for (int i = 1; i < (quantity + 1); i++)
		{
			// A jelenlegi sugar szoge balrol jobbra szamitva.
			Quaternion lineRotation =
				Quaternion.AngleAxis((angleBase * i), (rayOriginPoint.up));

			// A sugar kezdo es vegpontjai
			Vector3 rayOrigin = rayOriginPoint.position;
			Vector3 rayDirection = lineRotation * (-rayOriginPoint.right);

			// Letrehozza a sugarat es eltarolja hogy hozzaernek-e a falhoz.
			Physics.Raycast(
				rayOrigin,
				rayDirection,
				out raycastHit[i - 1],
				lineDistance,
				LayerMask.GetMask(rayLayerName));

			// Megrajzolja a sugarat. 
			rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(0, rayOrigin);
			rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(1, rayOrigin + rayDirection * lineDistance);
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
			rayHolders[i].GetComponent<LineRenderer>().startColor = Color.red;
			rayHolders[i].GetComponent<LineRenderer>().endColor = Color.white;

		}
	}

}
