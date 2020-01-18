using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{

    public string Key;

    void Start()
    {
        SetText();
        LocalizationManager.Instance.LanguageChangeEvent += OnLanguageChanged;
    }

    private void OnLanguageChanged(object sender, System.EventArgs e)
    {
        SetText();
    }

    private void SetText()
    {
        TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = LocalizationManager.Instance.GetLocalizedValue(Key);
    }
}
