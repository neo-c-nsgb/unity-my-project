using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string currentLanguage = "en";

    public TMPro.TMP_FontAsset fontCurrent;
    public TMPro.TMP_FontAsset fontEn;
    public TMPro.TMP_FontAsset fontFr;
    public TMPro.TMP_FontAsset fontDe;
    public TMPro.TMP_FontAsset fontZh;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLocalizedText(currentLanguage);
    }

    /// <summary>
    /// Loads and parses the CSV (from Resources/localization.csv) for the given language code.
    /// </summary>
    public void LoadLocalizedText(string language)
    {
        localizedText = new Dictionary<string, string>();
        currentLanguage = language;

        switch (language)
        {
            case "en":
                fontCurrent = fontEn;
                break;
            case "fr":
                fontCurrent = fontFr;
                break;
            case "de":
                fontCurrent = fontDe;
                break;
            case "zh":
                fontCurrent = fontZh;
                break;
        }

        UnityEngine.TextAsset csvAsset = Resources.Load<UnityEngine.TextAsset>("localization");
        if (csvAsset == null)
        {
            Debug.LogError("Localization CSV not found in Resources/localization.csv");
            return;
        }

        // Use CSVLoader to parse lines
        List<string[]> rows = CSVLoader.LoadCSV(csvAsset.text, ',');

        if (rows.Count < 2)
        {
            Debug.LogError("Localization CSV must have a header row + at least one data row.");
            return;
        }

        // First row = headers: key,en,fr,de,...
        string[] headers = rows[0];
        int langIndex = Array.IndexOf(headers, language);
        if (langIndex < 0)
        {
            Debug.LogError($"Language '{language}' not found in CSV headers.");
            return;
        }

        // Process each subsequent row
        for (int i = 1; i < rows.Count; i++)
        {
            string[] parts = rows[i];
            if (parts.Length > langIndex)
            {
                string key = parts[0].Trim();
                string value = parts[langIndex].Trim();
                if (!localizedText.ContainsKey(key))
                    localizedText.Add(key, value);
                else
                    localizedText[key] = value;
            }
        }

        isReady = true;
        UpdateAllLocalizedText();
    }

    /// <summary>
    /// Returns the localized string for the given key, or the key itself if not found.
    /// </summary>
    public string GetLocalizedValue(string key)
    {
        if (isReady && localizedText.TryGetValue(key, out var val))
            return val;
        return key;
    }

    /// <summary>
    /// Switches active language at runtime (e.g. "en", "fr", "de", "zh").
    /// </summary>
    public void SetLanguage(string langCode)
    {
        if (langCode == currentLanguage) return;
        LoadLocalizedText(langCode);

    }


    private void UpdateAllLocalizedText()
    {
        var activeTexts = UnityEngine.Object.FindObjectsByType<LocalizedText>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        foreach (var lt in activeTexts)
            lt.UpdateText();
    }

    public bool IsReady() => isReady;
    public string CurrentLanguage => currentLanguage;
}