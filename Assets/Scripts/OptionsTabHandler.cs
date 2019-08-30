using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionsTabHandler : MonoBehaviour
{
    [SerializeField] private GameObject TabPrefab;
    [SerializeField] private Slider TabNumberSlider;
    [SerializeField] private MenuController2 MenuController2;


    private List<GameObject> m_TabList = new List<GameObject>();
    private int m_TabCount = 0;
    private int m_ActiveTabIndex = 0;

    private const int s_TabSize = 100;


    public int ActiveTabIndex
    {
        get
        {
            return m_ActiveTabIndex;
        }

        set
        {
            // deactivate the previous tab
            m_TabList[m_ActiveTabIndex].GetComponent<PageTab>().ActivateTab(false);
            // and activate the current one
            m_TabList[value].GetComponent<PageTab>().ActivateTab(true);

            m_ActiveTabIndex = value;
            Master.Instance.CurrentEditConfigId = value;
            MenuController2.UpdateAllUIElement();
        }
    }

    public int TabCount
    {
        get
        {
            return m_TabCount;
        }

        set
        {
            if (m_TabCount != value)
            {
                for (int i = 0; i < value; i++)
                {
                    m_TabList[i].SetActive(true);
                }

                for (int i = value; i <= Master.MAX_CONFIGURATIONS; i++)
                {
                    m_TabList[i].SetActive(false);
                }

                if (ActiveTabIndex >= value)
                {
                    ActiveTabIndex = value - 1;
                }

                Master.Instance.PopulateConfigsUntil(value);
                ActiveTabIndex = value - 1;
                Master.Instance.RemoveConfigs(value);
            }
        }
    }

    public void UpdateTabCount()
    {
        TabCount = (int)TabNumberSlider.value;
    }
    // Use this for initialization
    private void Start()
    {
        for (int i = 0; i <= Master.MAX_CONFIGURATIONS; i++)
        {
            GameObject instance = Instantiate(TabPrefab, transform);
            PageTab pageTab = instance.GetComponent<PageTab>();
            pageTab.PageNumber = i + 1;
            pageTab.ClickedEvent += PageTab_ClickedEvent;
            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(i * s_TabSize, 0f, 0f);
            m_TabList.Add(instance);
            instance.SetActive(false);
        }
        m_TabList[0].SetActive(true);
        m_TabList[m_ActiveTabIndex].GetComponent<PageTab>().ActivateTab(true);
    }

    private void PageTab_ClickedEvent(object sender, EventArgs e)
    {
        PageTabEventArgs args = e as PageTabEventArgs;
        if (args != null)
        {
            ActiveTabIndex = args.PageNumber - 1;
        }
    }
}