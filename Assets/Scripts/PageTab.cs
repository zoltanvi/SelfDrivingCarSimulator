using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PageTab : MonoBehaviour {

    private TextMeshProUGUI m_TextMesh;
    private int m_PageNumber = 0;
    private static readonly Color m_DeactivatedColor = new Color(0.2f, 0.3f, 0.4f, 1f);
    private static readonly Color m_ActivatedColor = new Color(0.07f, 0.12f, 0.4f, 1f);



    public event EventHandler ClickedEvent;
        
    public int PageNumber
    {
        get
        {
            return m_PageNumber;
        }
        set
        {
            m_TextMesh.text = value.ToString();
            m_PageNumber = value;
        }
    }

    private void Awake()
    {
        m_TextMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (m_TextMesh == null)
        {
            Debug.LogError("PageTab.TextMesh not found!");
        }
    }

    public void OnClicked()
    {
        ClickedEvent?.Invoke(this, new PageTabEventArgs { PageNumber = m_PageNumber});
    }

    public void ActivateTab(bool activate)
    {
        if (activate)
        {
            GetComponentInChildren<Image>().color = m_ActivatedColor;
        }
        else
        {
            GetComponentInChildren<Image>().color = m_DeactivatedColor;
        }
    }

}

public class PageTabEventArgs : EventArgs
{
    public int PageNumber { get; set; }
}

