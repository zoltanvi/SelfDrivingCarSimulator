using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PopUpText : MonoBehaviour
{
    private float alpha;

    [SerializeField] private GameObject popUpCanvas;
    [SerializeField] private GameObject popUpPanel;
    [SerializeField] private GameObject popUpText;

    private Image panelImage;
    private TextMeshProUGUI cheatText;

    private Color tmpTextColor;
    private Color tmpPanelColor;

    private void Awake()
    {

        // cheatCanvas = GameObject.Find("PopUpCanvas");
        // panel = GameObject.Find("PopUpPanel");
        // text = GameObject.Find("PopUpText");


        panelImage = popUpPanel.GetComponent<Image>();
        cheatText = popUpText.GetComponent<TextMeshProUGUI>();

        alpha = 0;
        tmpTextColor = new Color();
        tmpPanelColor = new Color();
        tmpTextColor = cheatText.color;
        tmpPanelColor = panelImage.color;
		SetColors();
    }

	public void ShowPopUp(string message)
	{
        StopAllCoroutines();
        popUpCanvas.SetActive(true);
        alpha = 0;
        SetColors();

        //float boxWidth = (message.Length <= 30f) ? (message.Length * 25f) : ((message.Length * 20f) + 30f);
        //float boxWidth = Screen.width - 100;

		//popUpPanel.GetComponent<RectTransform>()
		//.sizeDelta = new Vector2(boxWidth, 60f);
		cheatText.text = message;
        StartCoroutine(FadeIn());		
	}

    private IEnumerator FadeIn()
    {
        while (alpha < 1)
        {
            alpha += 0.05f;
            SetColors();
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1f);

        while (alpha > 0)
        {
            alpha -= 0.05f;
            SetColors();
            yield return new WaitForSeconds(0.05f);
        }

        // final cleanup
        alpha = 0f;
        SetColors();
        popUpCanvas.SetActive(false);
    }

    private void SetColors()
    {
        tmpTextColor.a = alpha;
        tmpPanelColor.a = alpha;
        cheatText.color = tmpTextColor;
        panelImage.color = tmpPanelColor;
    }
}

