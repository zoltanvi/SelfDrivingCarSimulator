[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] Items;
}

[System.Serializable]
public class LocalizationItem
{
    public string Key;
    public string Value;
}
