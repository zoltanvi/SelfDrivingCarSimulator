using UnityEngine;
using TMPro;

public class UIPrinter : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI sensorText;
	[SerializeField] private TextMeshProUGUI fitnessText;
	[SerializeField] private TextMeshProUGUI wrongwayText;

	public string SensorDistances { get; set; }
	public double FitnessValue { get; set; }

	void Update () 
	{
		// Kiirja az erzekelok adatait, fitness erteket az autonak
		sensorText.text = SensorDistances;
		fitnessText.text = "Fitness: " + string.Format("{0:0.0000}", FitnessValue);

		// Ha az autohoz tartozo fitness kisebb mint 0, megjelenik a "!!!" felirat
		wrongwayText.text = (FitnessValue < 0) ? "!!!" : "";

	}
	
}
