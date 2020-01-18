using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpText : MonoBehaviour
{
    private float m_Alpha;

    [SerializeField] private GameObject m_PopUpCanvas;
    [SerializeField] private GameObject m_PopUpPanel;
    [SerializeField] private GameObject m_PopUpText;

    private Image m_PanelImage;
    private TextMeshProUGUI m_CheatText;

    private Color m_TmpTextColor;
    private Color m_TmpPanelColor;

    private void Awake()
    {
        m_PanelImage = m_PopUpPanel.GetComponent<Image>();
        m_CheatText = m_PopUpText.GetComponent<TextMeshProUGUI>();

        m_Alpha = 0;
        m_TmpTextColor = new Color();
        m_TmpPanelColor = new Color();
        m_TmpTextColor = m_CheatText.color;
        m_TmpPanelColor = m_PanelImage.color;
        SetColors();
    }

    public void ShowPopUp(string message)
    {
        StopAllCoroutines();
        m_PopUpCanvas.SetActive(true);
        m_Alpha = 0;
        SetColors();

        m_CheatText.text = message;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (m_Alpha < 1)
        {
            m_Alpha += 0.05f;
            SetColors();
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1f);

        while (m_Alpha > 0)
        {
            m_Alpha -= 0.05f;
            SetColors();
            yield return new WaitForSeconds(0.05f);
        }

        // final cleanup
        m_Alpha = 0f;
        SetColors();
        m_PopUpCanvas.SetActive(false);
    }

    private void SetColors()
    {
        m_TmpTextColor.a = m_Alpha;
        m_TmpPanelColor.a = m_Alpha;
        m_CheatText.color = m_TmpTextColor;
        m_PanelImage.color = m_TmpPanelColor;
    }
}