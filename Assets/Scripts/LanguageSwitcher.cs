using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    public void SetEnglish() => LocalizationManager.Instance.SetLanguage("en");
    public void SetFrench() => LocalizationManager.Instance.SetLanguage("fr");
    public void SetGerman() => LocalizationManager.Instance.SetLanguage("de");
    public void SetChinese() => LocalizationManager.Instance.SetLanguage("zh");
}