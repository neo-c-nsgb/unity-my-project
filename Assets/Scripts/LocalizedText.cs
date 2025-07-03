using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class LocalizedText : MonoBehaviour
{
    public string localizationKey;

    private Text uiText;
    private TextMeshProUGUI tmpText;


    void Awake()
    {
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (!LocalizationManager.Instance || !LocalizationManager.Instance.IsReady()) return;

        string value = LocalizationManager.Instance.GetLocalizedValue(localizationKey);

        if (uiText != null)
            uiText.text = value;

        else if (tmpText != null)
        { 
          tmpText.text = value;
          tmpText.font = LocalizationManager.Instance.fontCurrent;
        }
    }
}