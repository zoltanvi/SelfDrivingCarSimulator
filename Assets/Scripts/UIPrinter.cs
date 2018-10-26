using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIPrinter : MonoBehaviour
{

	[SerializeField] public TextMeshProUGUI GenerationText;
	[SerializeField] private TextMeshProUGUI remainingTimeText;
	[SerializeField] private TextMeshProUGUI freezeTimeText;
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

	private List<float> maxF;
	private List<float> medF;
	private readonly Color green = new Color(0f, 1f, 0f, 1f);
	private readonly Color red = new Color(1f, 0f, 0f, 1f);
	private int panelHeight;
	private int numLines;

	public float FitnessValue { get; set; }
	public string ConsoleMessage { get; set; }
	public int GenerationCount { get; set; }

	private void Start()
	{
		ConsoleMessage = string.Empty;
		SetPanelHeight();
	}

	private void Update()
	{
		maxF = Master.Instance.Manager.MaxFitness;
		medF = Master.Instance.Manager.MedianFitness;
		remainingTimeText.text = string.Format("{0:0.0} sec", Master.Instance.Manager.GlobalTimeLeft);
		freezeTimeText.text = string.Format("{0:0.0} sec", Master.Instance.Manager.FreezeTimeLeft);
		if (Master.Instance.Manager.WasItALoad)
		{
			GenerationText.text = string.Format("{0:0}", Master.Instance.Manager.Save.GenerationCount);
		}
		else
		{
			GenerationText.text = string.Format("{0:0}", GenerationCount);
		}

		fitnessText.text = string.Format("{0:0.00}", FitnessValue);


		consoleText.text = ConsoleMessage;

		if (Master.Instance.Manager.ManualControl)
		{
			creatureIDText.text = TextResources.GetValue("creature_ID_player");
		}
		else
		{
			creatureIDText.text = string.Format("#{0:0}", Master.Instance.Manager.BestCarId);
		}


		maxText.text = string.Format("{0:0.00}", (maxF.Count - 1 >= 0) ? maxF[maxF.Count - 1] : 0);
		medianText.text = string.Format("{0:0.00}", (medF.Count - 1 >= 0) ? medF[medF.Count - 1] : 0);
		mutationRateText.text = string.Format("{0}%", Master.Instance.Manager.MutationRate);
		populationText.text = string.Format("{0:0}", Master.Instance.Manager.CarCount);
		aliveCountText.text = Master.Instance.Manager.AliveCount.ToString();

		float prevMax = (maxF.Count - 2 >= 0) ? maxF[maxF.Count - 2] : 0;
		float currentMax = (maxF.Count - 1 >= 0) ? maxF[maxF.Count - 1] : 0;

		float prevMed = (medF.Count - 2 >= 0) ? medF[medF.Count - 2] : 0;
		float currentMed = (medF.Count - 1 >= 0) ? medF[medF.Count - 1] : 0;


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

		// If the player joined the game, the console will be disabled
		consolePanel.SetActive(!Master.Instance.Manager.ManualControl);
	}

	private void SetPanelHeight()
	{
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

}

