using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpText : MonoBehaviour
{
    private float fadeDuration = 0.3f;
    private float duration = 1f;
    private float lifeTime = 0f;
    private float alpha;

    private GameObject panel;
    private GameObject text;

    private Image panelImage;
    private TextMeshProUGUI cheatText;

    private Color tmpTextColor;
    private Color tmpPanelColor;
    void Start()
    {
        panel = GameObject.Find("CheatPanel");
        text = GameObject.Find("CheatText");

        panelImage = panel.GetComponent<Image>();
        cheatText = text.GetComponent<TextMeshProUGUI>();
        if (Master.Instance.CreateDemoSave)
        {
            cheatText.text = "Demo save mode enabled!";
        }
        else
        {
            cheatText.text = "Demo save mode disabled!";
        }

        alpha = 0;
        fadeDuration = Master.Instance.popupFadeTime;
        duration = Master.Instance.popupDurationTime;
        tmpTextColor = new Color();
        tmpPanelColor = new Color();
        tmpTextColor = cheatText.color;
        tmpPanelColor = panelImage.color;
    }


    void Update()
    {
        // Fade in
        if (lifeTime <= fadeDuration)
        {

            if (alpha < 1)
            {
                alpha += Time.deltaTime / fadeDuration;
            }

            // Fade out
        }
        else if (lifeTime > fadeDuration + duration && lifeTime <= (fadeDuration * 2) + duration)
        {

            if (alpha > 0)
            {
                alpha -= Time.deltaTime / fadeDuration;
            }

            // Destroys itself 
        }
        else if (lifeTime > (fadeDuration * 2) + duration)
        {
            alpha = 0f;
            // final cleanup
            tmpTextColor.a = 0f;
            tmpPanelColor.a = 0f;
            cheatText.color = tmpTextColor;
            panelImage.color = tmpPanelColor;

            Destroy(gameObject);
        }

        tmpTextColor.a = alpha;
        tmpPanelColor.a = alpha;
        cheatText.color = tmpTextColor;
        panelImage.color = tmpPanelColor;
        lifeTime += Time.deltaTime;

    }
}
