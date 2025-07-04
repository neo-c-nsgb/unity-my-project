// Assets/Scripts/TetrominoIndicator.cs
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TetrominoIndicator : MonoBehaviour
{
    [Tooltip("Reference to your TetrisController (auto-find if left null)")]
    public TetrisController controller;
    [Tooltip("Prefab: a simple Image with no child, anchored (0,0) pivot (0,0)")]
    public Image markerPrefab;
    public float markerHeight = 15f;

    private Image[] markers;
    private RectTransform rt;
    private bool initialized = false;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        // auto-find controller if not set
        if (controller == null)
        {
            controller = Object.FindFirstObjectByType<TetrisController>();
            if (controller == null)
                Debug.LogError("[Indicator] No TetrisController found on scene!");
        }
    }

    /// <summary>
    /// Creates marker pool based on current grid width.
    /// </summary>
    private void InitMarkers()
    {
        int maxColumns = (controller != null) ? controller.GridWidth : 0;
        markers = new Image[maxColumns];
        for (int i = 0; i < maxColumns; i++)
        {
            var m = Instantiate(markerPrefab, rt, false);
            m.gameObject.SetActive(false);
            markers[i] = m;
        }
        initialized = true;
    }

    /// <summary>
    /// Call this any time the piece moves or rotates.
    /// </summary>
    public void UpdateIndicator()
    {
        if (controller == null) return;
        if (!initialized) InitMarkers();

        // get distinct occupied columns
        var cells = controller.CurrentShapeCells;
        var pos = controller.CurrentShapePos;
        float size = controller.CellSize;

        var xs = cells
            .Select(c => c.x + pos.x)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();



        for (int i = 0; i < markers.Length; i++)
        {
            if (i < xs.Length)
            {
                int col = xs[i];
                var m = markers[i];
                m.gameObject.SetActive(true);
                var mRT = m.rectTransform;
                mRT.anchorMin = mRT.anchorMax = new Vector2(0, 0);
                mRT.pivot = new Vector2(0, 0);
                mRT.anchoredPosition = new Vector2(col * size, 0);
                mRT.sizeDelta = new Vector2(size, size * markerHeight);


            }
            else
            {
                markers[i].gameObject.SetActive(false);
            }
        }
    }
}
