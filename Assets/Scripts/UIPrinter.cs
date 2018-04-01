using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class UIPrinter : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI remainingTimeText;
	[SerializeField] private TextMeshProUGUI freezeTimeText;
	[SerializeField] private TextMeshProUGUI generationText;
	[SerializeField] private TextMeshProUGUI fitnessText;
	[SerializeField] private TextMeshProUGUI creatureIDText;
	[SerializeField] private TextMeshProUGUI averageText;
	[SerializeField] private TextMeshProUGUI medianText;
	[SerializeField] private TextMeshProUGUI mutationRateText;
	[SerializeField] private TextMeshProUGUI populationText;
	[SerializeField] private TextMeshProUGUI averageDifferenceText;
	[SerializeField] private TextMeshProUGUI medianDifferenceText;
	[SerializeField] private TextMeshProUGUI aliveCountText;

	private List<double> avgF;
	private List<double> medF;
	private Color green = new Color(0f, 1f, 0f, 1f);
	private Color red = new Color(1f, 0f, 0f, 1f);

	public double FitnessValue { get; set; }

	void Update () 
	{
		avgF = CarGameManager.Instance.avgFitness;
		medF = CarGameManager.Instance.medianFitness;
		remainingTimeText.text = string.Format("{0:0.0} sec", CarGameManager.Instance.globalTimeLeft);
		freezeTimeText.text = string.Format("{0:0.0} sec", CarGameManager.Instance.freezeTimeLeft);
		generationText.text = string.Format("{0:0}", CarGameManager.Instance.GenerationCount);
		fitnessText.text = string.Format("{0:0.00}", FitnessValue);
		creatureIDText.text = string.Format("#{0:0}", CarGameManager.Instance.bestCarIndex);
		averageText.text = string.Format("{0:0.00}", (avgF.Count - 1 >= 0) ? avgF[avgF.Count - 1] : 0);
		medianText.text = string.Format("{0:0.00}", (medF.Count - 1 >= 0) ? medF[medF.Count - 1] : 0);
		mutationRateText.text = string.Format("{0:0}%", CarGameManager.Instance.MutationRate);
		populationText.text = string.Format("{0:0}", CarGameManager.Instance.CarCount);
		aliveCountText.text = CarGameManager.Instance.carsAliveCount.ToString();

		double prevAvg = (avgF.Count - 2 >= 0) ? avgF[avgF.Count - 2] : 0;
		double currentAvg = (avgF.Count - 1 >= 0) ? avgF[avgF.Count - 1] : 0;

		double prevMed = (medF.Count - 2 >= 0) ? medF[medF.Count - 2] : 0;
		double currentMed = (medF.Count - 1 >= 0) ? medF[medF.Count - 1] : 0;


		if ((prevAvg - currentAvg) <= 0)
		{
			averageDifferenceText.color = green;
			averageDifferenceText.text = string.Format("+{0:0.0}", (currentAvg - prevAvg));
		}
		else
		{
			averageDifferenceText.color = red;
			averageDifferenceText.text = string.Format("-{0:0.0}", (prevAvg - currentAvg));
		}

		if ((prevMed - currentMed) <= 0)
		{
			medianDifferenceText.color = green;
			medianDifferenceText.text = string.Format("+{0:0.0}", (currentMed - prevMed));
		}
		else
		{
			medianDifferenceText.color = red;
			medianDifferenceText.text = string.Format("-{0:0.0}", (prevMed - currentMed));
		}

	}
	
}
