// Assets/Scripts/TetrominoIndicator.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TetrominoIndicator : MonoBehaviour
{
    [Tooltip("The RectTransform of your Tetris grid; used for sizing.")]
    public RectTransform gridContainer;

    [Tooltip("A thin UI Image prefab to mark each column.")]
    public RectTransform markerPrefab;

    // internally tracked markers
    private readonly List<RectTransform> markers = new List<RectTransform>();

    private TetrisController tetris;

    void Awake()
    {
        tetris = Object.FindFirstObjectByType<TetrisController>();
        if (tetris == null)
            Debug.LogError("[Indicator] No TetrisController found.");

        if (gridContainer == null)
            Debug.LogError("[Indicator] gridContainer not assigned!");

        if (markerPrefab == null)
            Debug.LogError("[Indicator] markerPrefab not assigned!");
    }

    /// <summary>
    /// Call whenever you need to redraw the landing indicator.
    /// </summary>
    /// <param name="shapeColumns">Absolute columns the current shape covers</param>
    public void UpdateIndicator(IEnumerable<int> shapeColumns)
    {
        // 1) Recompute sizes now that gridContainer has been laid out
        float cellSize = tetris.CellSize;
        float gridHeightPx = gridContainer.rect.height;

        // 2) Clear any old markers
        foreach (var m in markers)
            if (m != null) Destroy(m.gameObject);
        markers.Clear();

        // 3) Instantiate new markers under THIS panel
        foreach (var col in shapeColumns)
        {
            var m = Instantiate(markerPrefab, transform, false);
            m.gameObject.SetActive(true);

            // anchor bottom-left
            m.anchorMin = m.anchorMax = Vector2.zero;
            m.pivot = Vector2.zero;

            // size = one column wide, full grid height
            m.sizeDelta = new Vector2(cellSize, gridHeightPx);
            m.anchoredPosition = new Vector2(col * cellSize, 0f);

            markers.Add(m);
        }
    }
}
