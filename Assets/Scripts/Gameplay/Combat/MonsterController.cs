// Assets/Scripts/MonsterController.cs
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MonsterController : MonoBehaviour
{
    /// <summary>Data asset defining this monster’s stats & visual path.</summary>
    public MonsterData data { get; private set; }

    /// <summary>Which column (0 = leftmost) this monster occupies.</summary>
    public int column { get; private set; }

    RectTransform containerRT;
    TetrisController tetris;
    GameObject visualInstance;

    /// <summary>
    /// Initialize the monster: position, parenting, visual, and draw order.
    /// </summary>
    public void Initialize(MonsterData data, int col, TetrisController tetrisController)
    {
        this.data = data;
        this.column = col;
        this.tetris = tetrisController;
        containerRT = GetComponent<RectTransform>();

        float cell = tetris.CellSize;

        // 1) Position container at bottom-left of its cell, above the top row
        containerRT.anchorMin = containerRT.anchorMax = Vector2.zero;
        containerRT.pivot = new Vector2(0f, 0f);
        containerRT.anchoredPosition = new Vector2(col * cell, tetris.GridHeight * cell);

        // 2) Parent under gridContainer
        transform.SetParent(tetris.gridContainer, false);

        // 3) Instantiate the full-fidelity visual prefab as a child
        var prefab = data.Prefab;
        if (prefab == null)
        {
            Debug.LogError($"MonsterController: prefab not found for '{data.monsterId}' at '{data.monsterPrefabPath}'");
            return;
        }
        visualInstance = Instantiate(prefab, transform, false);
        var vRT = visualInstance.GetComponent<RectTransform>();

        // 4) Align the child for center‐in‐cell and lift 0.5 cell up:
        vRT.anchorMin = vRT.anchorMax = Vector2.zero;
        vRT.pivot = new Vector2(0.5f, 0f);            // bottom-center pivot
        vRT.sizeDelta = new Vector2(cell, cell);          // one cell size
        vRT.anchoredPosition = new Vector2(cell * 0.5f, cell * 0.5f);

        // 5) Reorder siblings: monster under hero, above all blocks
        var heroRT = tetris.Hero.GetComponent<RectTransform>();
        // a) bring hero to the very top
        heroRT.SetAsLastSibling();
        // b) place monster just below hero
        containerRT.SetSiblingIndex(heroRT.GetSiblingIndex() - 1);

        // 6) Initial facing
        UpdateFacing();
    }

    /// <summary>
    /// Called each gravity pass; drops the container down onto the settled blocks.
    /// </summary>
    public void ApplyGravity()
    {
        float cell = tetris.CellSize;
        int height = tetris.GetColumnHeight(column);
        containerRT.anchoredPosition = new Vector2(column * cell, height * cell);
    }

    void LateUpdate()
    {
        UpdateFacing();
    }

    /// <summary>
    /// Flips the visual child around its bottom-center pivot to face the hero.
    /// </summary>
    void UpdateFacing()
    {
        if (visualInstance == null || tetris.Hero == null) return;

        bool faceRight = tetris.Hero.CurrentColumn > column;
        var vRT = visualInstance.GetComponent<RectTransform>();
        var s = vRT.localScale;
        s.x = Mathf.Abs(s.x) * (faceRight ? 1f : -1f);
        vRT.localScale = s;
    }
}
