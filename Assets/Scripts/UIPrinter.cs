using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIPrinter : MonoBehaviour
{

    public TextMeshProUGUI GenerationNumber;
    [SerializeField] private TextMeshProUGUI m_SimulationNumber;
    [SerializeField] private TextMeshProUGUI m_RemainingTime;
    [SerializeField] private TextMeshProUGUI m_FreezeTime;
    [SerializeField] private TextMeshProUGUI m_PopulationNumber;
    [SerializeField] private TextMeshProUGUI m_CreatureId;
    [SerializeField] private TextMeshProUGUI m_FitnessNumber;
    [SerializeField] private TextMeshProUGUI m_MaxFitnessNumber;
    [SerializeField] private TextMeshProUGUI m_MedianFitnessNumber;
    [SerializeField] private TextMeshProUGUI m_MaxDifferenceNumber;
    [SerializeField] private TextMeshProUGUI m_MedianDifferenceNumber;
	[SerializeField] private TextMeshProUGUI m_ConsoleText;
	[SerializeField] private GameObject m_ConsolePanel;
	[SerializeField] private GameObject m_DemoMode;

    private List<float> m_MaxFitnessList;
	private List<float> m_MedianFitnessList;
	private readonly Color s_Green = new Color(0f, 1f, 0f, 1f);
	private readonly Color s_Red = new Color(1f, 0f, 0f, 1f);
	private int m_PanelHeight;
	private int m_NumberOfLines;

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
        m_SimulationNumber.text = string.Format("{0} / {1}", Master.Instance.CurrentConfigId + 1, Master.Instance.Configurations.Count);
        
        if (Master.Instance.CurrentConfiguration.DemoMode)
        {
            m_DemoMode.SetActive(true);
        }
        else
        {
            m_DemoMode.SetActive(false);
        }

        m_MaxFitnessList = Master.Instance.Manager.MaxFitness;
		m_MedianFitnessList = Master.Instance.Manager.MedianFitness;
		m_RemainingTime.text = string.Format("{0:0.0} s", Master.Instance.Manager.GlobalTimeLeft);
		m_FreezeTime.text = string.Format("{0:0.0} s", Master.Instance.Manager.FreezeTimeLeft);
		if (Master.Instance.Manager.IsLoad)
		{
            if (Master.Instance.CurrentConfiguration.StopConditionActive)
            {
			    GenerationNumber.text = string.Format("{0:0} / {1}", Master.Instance.Manager.Save.GenerationCount, Master.Instance.CurrentConfiguration.StopGenerationNumber);
            }
            else
            {
                GenerationNumber.text = string.Format("{0:0}", Master.Instance.Manager.Save.GenerationCount);
            }
        }
		else
		{
            if (Master.Instance.CurrentConfiguration.StopConditionActive)
            {
                GenerationNumber.text = string.Format("{0:0} / {1}", GenerationCount, Master.Instance.CurrentConfiguration.StopGenerationNumber);
            }
            else
            {
                GenerationNumber.text = string.Format("{0:0}", GenerationCount);
            }
        }

		m_FitnessNumber.text = string.Format("{0:0.00}", FitnessValue);


		m_ConsoleText.text = ConsoleMessage;

		if (Master.Instance.Manager.ManualControl)
		{
			m_CreatureId.text = LocalizationManager.Instance.GetLocalizedValue("hud_creature_player");
		}
		else
		{
			m_CreatureId.text = string.Format("#{0:0}", Master.Instance.Manager.BestCarId);
		}


		m_MaxFitnessNumber.text = string.Format("{0:0.00}", (m_MaxFitnessList.Count - 1 >= 0) ? m_MaxFitnessList[m_MaxFitnessList.Count - 1] : 0);
		m_MedianFitnessNumber.text = string.Format("{0:0.00}", (m_MedianFitnessList.Count - 1 >= 0) ? m_MedianFitnessList[m_MedianFitnessList.Count - 1] : 0);
		m_PopulationNumber.text = string.Format("{0} / {1:0}", Master.Instance.Manager.AliveCount, Master.Instance.Manager.Configuration.CarCount);
		
		float prevMax = (m_MaxFitnessList.Count - 2 >= 0) ? m_MaxFitnessList[m_MaxFitnessList.Count - 2] : 0;
		float currentMax = (m_MaxFitnessList.Count - 1 >= 0) ? m_MaxFitnessList[m_MaxFitnessList.Count - 1] : 0;

		float prevMed = (m_MedianFitnessList.Count - 2 >= 0) ? m_MedianFitnessList[m_MedianFitnessList.Count - 2] : 0;
		float currentMed = (m_MedianFitnessList.Count - 1 >= 0) ? m_MedianFitnessList[m_MedianFitnessList.Count - 1] : 0;


		if ((prevMax - currentMax) <= 0)
		{
			m_MaxDifferenceNumber.color = s_Green;
            m_MaxDifferenceNumber.text = string.Format("+{0:0.0}", (currentMax - prevMax));
		}
		else
		{
            m_MaxDifferenceNumber.color = s_Red;
            m_MaxDifferenceNumber.text = string.Format("-{0:0.0}", (prevMax - currentMax));
		}

		if ((prevMed - currentMed) <= 0)
		{
			m_MedianDifferenceNumber.color = s_Green;
            m_MedianDifferenceNumber.text = string.Format("+{0:0.0}", (currentMed - prevMed));
		}
		else
		{
            m_MedianDifferenceNumber.color = s_Red;
            m_MedianDifferenceNumber.text = string.Format("-{0:0.0}", (prevMed - currentMed));
		}

		// If the player joined the game, the console will be disabled
		m_ConsolePanel.SetActive(!Master.Instance.Manager.ManualControl);
	}

	private void SetPanelHeight()
	{
		if (Master.Instance.Manager.ManualControl)
		{
			m_PanelHeight = 0;
		}
		else
		{
			if (Master.Instance.Manager.Configuration.Navigator)
			{
				m_NumberOfLines = Master.Instance.Manager.CarSensorCount + 4;
			}
			else
			{
				m_NumberOfLines = Master.Instance.Manager.CarSensorCount + 1;
			}
			m_PanelHeight = (17 * m_NumberOfLines) + 5;
		}

		m_ConsolePanel.GetComponent<RectTransform>().sizeDelta = new Vector2(100, m_PanelHeight);
	}

}

