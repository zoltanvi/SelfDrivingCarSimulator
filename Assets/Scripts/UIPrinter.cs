using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPrinter : MonoBehaviour {

	#region Variables


	[SerializeField] private TextMeshProUGUI sensorText;
	[SerializeField] private TextMeshProUGUI fitnessText;
	[SerializeField] private TextMeshProUGUI wrongwayText;

	public string SensorDistances { get; set; }
	public double FitnessValue { get; set; }

	#endregion

	#region Methods
	void Update () 
	{
		sensorText.text = SensorDistances;

		fitnessText.text = "Fitness: " + string.Format("{0:0.0000}", FitnessValue);

		// Ha a rajttol visszafele megy az auto, megjelenik a WRONG WAY felirat.
		if (FitnessValue < 0)
		{
			wrongwayText.text = "!!!";
		}
		else
		{
			wrongwayText.text = "";
		}

	}

	#endregion
	
}
