using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class UIPrinter : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI remainingTimeText;
	[SerializeField] private TextMeshProUGUI freezeTimeText;
	[SerializeField] public TextMeshProUGUI generationText;
	[SerializeField] private TextMeshProUGUI fitnessText;
	[SerializeField] private TextMeshProUGUI creatureIDText;
	[SerializeField] private TextMeshProUGUI maxText;
	[SerializeField] private TextMeshProUGUI medianText;
	[SerializeField] private TextMeshProUGUI mutationRateText;
	[SerializeField] private TextMeshProUGUI populationText;
	[SerializeField] private TextMeshProUGUI maxDifferenceText;
	[SerializeField] private TextMeshProUGUI medianDifferenceText;
	[SerializeField] private TextMeshProUGUI aliveCountText;
	[SerializeField] private TextMeshProUGUI consoleText;
	[SerializeField] private GameObject consolePanel;

	// Maximum Fitness
	private List<double> maxF;
	private List<double> medF;

	private Color green = new Color(0f, 1f, 0f, 1f);
	private Color red = new Color(1f, 0f, 0f, 1f);


	private int panelHeight;
	private int numLines;


	public double FitnessValue { get; set; }
	public string ConsoleMessage { get; set; }
	public int GenerationCount { get; set; }
	private bool controlledByPlayer = false;

	void Start()
	{
		controlledByPlayer = Master.Instance.Manager.ManualControl;
		ConsoleMessage = "";
		panelHeight = 0;
		// TODO: változik majd a navigátorral
		if (Master.Instance.Manager.ManualControl)
		{
			panelHeight = 0;
		}
		else
		{
			if (Master.Instance.Manager.Navigator)
			{
				numLines = Master.Instance.Manager.CarSensorCount + 4;
			}
			else
			{
				numLines = Master.Instance.Manager.CarSensorCount + 1;
			}
			panelHeight = (17 * numLines) + 5;
		}

		consolePanel.GetComponent<RectTransform>().sizeDelta = new Vector2(100, panelHeight);
	}
	void Update()
	{
		maxF = Master.Instance.Manager.maxFitness;
		medF = Master.Instance.Manager.medianFitness;
		remainingTimeText.text = string.Format("{0:0.0} sec", Master.Instance.Manager.globalTimeLeft);
		freezeTimeText.text = string.Format("{0:0.0} sec", Master.Instance.Manager.freezeTimeLeft);
		if (Master.Instance.Manager.wasItALoad)
		{
			generationText.text = string.Format("{0:0}", Master.Instance.Manager.Save.GenerationCount);
		}
		else
		{
			generationText.text = string.Format("{0:0}", GenerationCount);
		}

		fitnessText.text = string.Format("{0:0.00}", FitnessValue);


		consoleText.text = ConsoleMessage;

		if (controlledByPlayer)
		{
			creatureIDText.text = "PLAYER";
		}
		else
		{
			creatureIDText.text = string.Format("#{0:0}", Master.Instance.Manager.bestCarID);
		}


		maxText.text = string.Format("{0:0.00}", (maxF.Count - 1 >= 0) ? maxF[maxF.Count - 1] : 0);
		medianText.text = string.Format("{0:0.00}", (medF.Count - 1 >= 0) ? medF[medF.Count - 1] : 0);
		mutationRateText.text = string.Format("{0}%", Master.Instance.Manager.MutationRate);
		populationText.text = string.Format("{0:0}", Master.Instance.Manager.CarCount);
		aliveCountText.text = Master.Instance.Manager.AliveCount.ToString();

		double prevMax = (maxF.Count - 2 >= 0) ? maxF[maxF.Count - 2] : 0;
		double currentMax = (maxF.Count - 1 >= 0) ? maxF[maxF.Count - 1] : 0;

		double prevMed = (medF.Count - 2 >= 0) ? medF[medF.Count - 2] : 0;
		double currentMed = (medF.Count - 1 >= 0) ? medF[medF.Count - 1] : 0;


		if ((prevMax - currentMax) <= 0)
		{
			maxDifferenceText.color = green;
			maxDifferenceText.text = string.Format("+{0:0.0}", (currentMax - prevMax));
		}
		else
		{
			maxDifferenceText.color = red;
			maxDifferenceText.text = string.Format("-{0:0.0}", (prevMax - currentMax));
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
