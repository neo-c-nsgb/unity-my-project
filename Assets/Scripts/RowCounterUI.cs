// Assets/Scripts/RowCounterUI.cs
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class RowCounterUI : MonoBehaviour
{
    private TMP_Text _text;
    private GameManager _gm;

    void Awake()
    {
        // Cache the Text component
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        // Lookup GameManager after its Awake has run
        _gm = GameManager.Instance ?? Object.FindFirstObjectByType<GameManager>();
        if (_gm == null)
        {
            Debug.LogError("RowCounterUI: no GameManager found in scene!");
            enabled = false;
            return;
        }

        // Subscribe to win event
        _gm.OnWin += OnWin;

        // Initial display
        UpdateText();
    }

    void OnDestroy()
    {
        if (_gm != null)
            _gm.OnWin -= OnWin;
    }

    void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (_gm == null) return;
        _text.text = $"{_gm.RowsCleared}/{_gm.WinRowCount}";
    }

    private void OnWin()
    {
        if (_gm == null) return;
        _text.text = $"{_gm.RowsCleared}/{_gm.WinRowCount} — You Win!";
    }
}
