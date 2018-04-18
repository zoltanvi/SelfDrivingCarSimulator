using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class UIPrinter : MonoBehaviour
{

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
	[SerializeField] private TextMeshProUGUI consoleText;
	[SerializeField] private GameObject consolePanel;


	private List<double> avgF;
	private List<double> medF;
	private Color green = new Color(0f, 1f, 0f, 1f);
	private Color red = new Color(1f, 0f, 0f, 1f);
	private int panelHeight;
	private int numLines;


	public double FitnessValue { get; set; }
	public string ConsoleMessage { get; set; }

	private bool controlledByPlayer = false;

	void Start()
	{
		controlledByPlayer = GameManager.Instance.manualControl;
		ConsoleMessage = "";
		panelHeight = 0;
	}
	void Update()
	{
		avgF = GameManager.Instance.avgFitness;
		medF = GameManager.Instance.medianFitness;
		remainingTimeText.text = string.Format("{0:0.0} sec", GameManager.Instance.globalTimeLeft);
		freezeTimeText.text = string.Format("{0:0.0} sec", GameManager.Instance.freezeTimeLeft);
		generationText.text = string.Format("{0:0}", GameManager.Instance.GenerationCount);
		fitnessText.text = string.Format("{0:0.00}", FitnessValue);


		consoleText.text = ConsoleMessage;
		numLines = ConsoleMessage.Split('\n').Length;
			
		if (ConsoleMessage.Length == 0)
		{
			panelHeight = 0;
		}
		else
		{
			panelHeight = (15 * numLines) + 5;
		}


		consolePanel.GetComponent<RectTransform>().sizeDelta = new Vector2(100, panelHeight);
		ConsoleMessage = numLines.ToString();

		if (controlledByPlayer)
		{
			creatureIDText.text = "PLAYER";
		}
		else
		{
			creatureIDText.text = string.Format("#{0:0}", GameManager.Instance.bestCarIndex);
		}


		averageText.text = string.Format("{0:0.00}", (avgF.Count - 1 >= 0) ? avgF[avgF.Count - 1] : 0);
		medianText.text = string.Format("{0:0.00}", (medF.Count - 1 >= 0) ? medF[medF.Count - 1] : 0);
		mutationRateText.text = string.Format("{0:0}%", GameManager.Instance.MutationRate);
		populationText.text = string.Format("{0:0}", GameManager.Instance.CarCount);
		aliveCountText.text = GameManager.Instance.carsAliveCount.ToString();

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
