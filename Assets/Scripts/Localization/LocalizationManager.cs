/*
Copyright (C) 2021 zoltanvi

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LocalizationManager : MonoBehaviour
{

    private Dictionary<string, string> m_LocalizedText;

    public static LocalizationManager Instance;
    private readonly string missingTextString = "?m?";

    public event EventHandler LanguageChangeEvent;
    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        // Default language is english
        LoadLocalizedText("language_en");
    }

    public void LoadLocalizedText(string fileName)
    {
        m_LocalizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            List<LocalizationItem> itemList = ConfigReader.ReadLocalizationData(filePath);

            foreach (var localizationItem in itemList)
            {
                m_LocalizedText.Add(localizationItem.Key, localizationItem.Value);
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
        if (!m_LocalizedText.TryGetValue(key, out result))
        {
            result = missingTextString;
        }

        return result;
    }
}
