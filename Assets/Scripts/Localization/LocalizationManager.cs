using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LocalizationManager : MonoBehaviour {

    private Dictionary<string, string> m_LocalizedText;

    public static LocalizationManager Instance;
    private readonly string missingTextString = "?m?";

    public event EventHandler LanguageChangeEvent;
	void Awake() {
		
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
	}
    private void Start()
    {
        // Default language is english
        LoadLocalizedText("language_en.json");
    }

    public void LoadLocalizedText(string fileName)
    {
        m_LocalizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string jsonBlob = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(jsonBlob);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                m_LocalizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            LanguageChangeEvent?.Invoke(this, EventArgs.Empty);

            Debug.Log($"Data loaded, dictionary contains: {m_LocalizedText.Count} entries.");
        }
        else
        {
            Debug.LogError($"Cannot find the language file {fileName}!");
        }
    }

    public string GetLocalizedValue(string key)
    {
        string result;
        if(!m_LocalizedText.TryGetValue(key, out result))
        {
            result = missingTextString;
        }

        return result;
    }
}
