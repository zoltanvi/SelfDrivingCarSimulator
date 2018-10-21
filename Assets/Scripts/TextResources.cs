using System;
using System.Xml;
using System.IO;
using UnityEngine;

public static class TextResources
{

    private static GameLanguage m_language = GameLanguage.ENGLISH;
    public const string versionNumber = "4.0";

    public static GameLanguage Language
    {
        get
        {
            return m_language;
        }

        set
        {
            m_language = value;
        }
    }

    private static string GetXmlLangName(GameLanguage lang)
    {
        string value = "";
        switch (lang)
        {
            case GameLanguage.ENGLISH:
                value = "English";
                break;

            case GameLanguage.HUNGARIAN:
                value = "Hungarian";
                break;
            default:
                value = "English";
                break;
        }
        return value;
    }


    public static string GetValue(string key)
    {
        string value = "";
        XmlDocument xmlDocument = new XmlDocument();
        TextAsset asset = Resources.Load("LanguageResources") as TextAsset;

        xmlDocument.LoadXml(asset.text);

        string language = GetXmlLangName(m_language);

        try
        {
            XmlNode selectedNode = xmlDocument.DocumentElement.SelectSingleNode("/Languages/" + language + "/string[@name='" + key + "']");
            value = selectedNode.InnerText;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            throw;
        }

        return value;
    }

}

public enum GameLanguage
{
    HUNGARIAN,
    ENGLISH
}