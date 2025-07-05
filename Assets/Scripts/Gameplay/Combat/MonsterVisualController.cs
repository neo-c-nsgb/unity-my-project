// Assets/Scripts/MonsterVisualController.cs
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MonsterVisualController : MonoBehaviour
{
    [Tooltip("If left null, finds the first TetrisController in scene.")]
    public TetrisController tetrisController;

    [Tooltip("If left null, finds the first HeroController in scene.")]
    public HeroController heroController;

    [Header("Sizing")]
    [Tooltip("Multiplier of CellSize for this visual’s width/height.")]
    public float sizeMultiplier = 1f;

    [Tooltip("Extra vertical offset in cell-units above the base line.")]
    public float verticalOffset = 0f;

    // internal
    RectTransform rectTransform;
    float baseCellSize;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // ensure we can find controllers
        if (tetrisController == null)
            tetrisController = Object.FindFirstObjectByType<TetrisController>();
        if (heroController == null)
            heroController = Object.FindFirstObjectByType<HeroController>();

        // configure pivot/anchors so flip is bottom-center
        rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0f);
    }

    void Start()
    {
        // cache the cell size
        baseCellSize = tetrisController.CellSize;
        UpdateSizeAndPosition();
    }

    void LateUpdate()
    {
        // 1) Flip toward hero
        if (heroController != null)
        {
            var monster = GetComponentInParent<MonsterController>();
            if (monster != null)
            {
                bool faceRight = heroController.CurrentColumn > monster.column;
                var s = rectTransform.localScale;
                s.x = Mathf.Abs(s.x) * (faceRight ? 1f : -1f);
                rectTransform.localScale = s;
            }
        }

        // 2) In case cell-size changed (e.g. resizing screen), keep size & pos correct
        if (!Mathf.Approximately(baseCellSize, tetrisController.CellSize))
        {
            baseCellSize = tetrisController.CellSize;
            UpdateSizeAndPosition();
        }
    }

    private void UpdateSizeAndPosition()
    {
        float cs = baseCellSize * sizeMultiplier;

        // 1) Size the visual
        rectTransform.sizeDelta = new Vector2(cs, cs);

        // 2) Position it so that its bottom-center sits on the column baseline
        //    The container’s anchoredPosition is already at (col*cellSize, h*cellSize).
        //    Here we offset horizontally by half a cell, and vertically by verticalOffset.
        float x = 0.5f * baseCellSize;
        float y = verticalOffset * baseCellSize;
        rectTransform.anchoredPosition = new Vector2(x, y);

        // Reset any unintended scale drift
        var s = rectTransform.localScale;
        s.y = Mathf.Abs(s.y);
        rectTransform.localScale = s;
    }
}
