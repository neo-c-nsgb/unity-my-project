// Assets/Scripts/TetrisController.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetrisController : MonoBehaviour
{
    [Header("Grid Settings")]
    public RectTransform gridContainer;
    public float topMargin = 50f, bottomMargin = 50f;
    public float clearDelay = 0.3f;

    [Header("Tetromino Palette")]
    public TetrominoLibrary library;

    [Header("Prefabs")]
    public GameObject blockPrefab;
    //public RectTransform heroPrefab;

    [Header("Indicator")]
    [Tooltip("Assign the TetrominoIndicator in the Inspector")]
    public TetrominoIndicator indicator;



    private int columns, rows;
    public float CellSize { get; private set; }

    public int GridWidth => columns;
    public int GridHeight => rows;

    private HeroController hero;

    /// <summary>
    /// Exposes the current HeroController for other systems.
    /// </summary>
    public HeroController Hero => hero;

    public MonsterManager monsterManager;

    /// <summary>
    /// Call once to tell the controller which HeroController to use.
    /// </summary>
    public void RegisterHero(HeroController hc)
    {
        hero = hc;
    }

    private UIBlock[,] grid;
    private List<Vector2Int> shapeCells;
    private List<UIBlock> shapeBlocks;
    private Vector2Int shapePos;
    private TetrominoData currentData;
    private bool shapeActive;
    private bool hasWon;

    public List<Vector2Int> CurrentShapeCells => shapeCells;
    public Vector2Int CurrentShapePos => shapePos;

    void Start()
    {
        GameManager.Instance.OnWin += () => hasWon = true;
        columns = GameManager.Instance.GridWidth;
        rows = GameManager.Instance.GridHeight;

        var pRT = gridContainer.parent as RectTransform;
        float availH = Mathf.Max(0, pRT.rect.height - topMargin - bottomMargin);
        CellSize = availH / rows;
        gridContainer.sizeDelta = new Vector2(CellSize * columns, availH);
        gridContainer.anchoredPosition = new Vector2(0, (bottomMargin - topMargin) * .5f);

        grid = new UIBlock[columns, rows];

       /* if (heroPrefab != null)
        {
            var go = Instantiate(heroPrefab, gridContainer, false);
            hero = go.GetComponent<HeroController>();
            hero.tetrisController = this;
            hero.startColumn = columns / 2;
            hero.UpdatePosition();
        }
       */
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (!shapeActive) return;
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.leftArrowKey.wasPressedThisFrame) TryMove(Vector2Int.left);
        if (kb.rightArrowKey.wasPressedThisFrame) TryMove(Vector2Int.right);
        if (kb.upArrowKey.wasPressedThisFrame) TryRotate();
        if (kb.downArrowKey.wasPressedThisFrame) HardDrop();
    }

    public int GetColumnHeight(int c)
    {
        for (int y = rows - 1; y >= 0; y--)
            if (grid[c, y] != null)
                return y + 1;
        return 0;
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (hasWon) yield break;
            library.RefreshPool();
            SpawnShapeFromData();
            shapeActive = true;

            while (CanPlace(shapeCells, shapePos + Vector2Int.down))
                yield return null;

            LockShape();
            shapeActive = false;

            if (monsterManager != null)
            {
                // collect the absolute columns this shape covers
                var shapeCols = new List<int>();
                foreach (var c in shapeCells)
                    shapeCols.Add(shapePos.x + c.x);

                // distinctify and convert to array
                int[] cols = shapeCols.Distinct().ToArray();
                monsterManager.KillMonstersInColumns(cols);
            }

            // --- Crush Check ---
            if (hero != null)
            {
                int hc = hero.CurrentColumn;
                foreach (var c in shapeCells)
                {
                    if (shapePos.x + c.x == hc)
                    {
                        hero.gameObject.SetActive(false);
                        // apply gravity to drop blocks into hero spot
                        ApplyGravityToAllColumns();
                        Canvas.ForceUpdateCanvases();
                        // short pause to show crash
                        yield return new WaitForSeconds(clearDelay);
                        Debug.Log("💥 Hero crushed! Game Over");
                        yield break;
                    }
                }
            }

            int cleared;
            do
            {
                ApplyGravityToAllColumns();
                hero?.UpdatePosition();

                cleared = ClearFullRowsOnce();
                hero?.UpdatePosition();
                Canvas.ForceUpdateCanvases();

                if (cleared > 0)
                    GameManager.Instance.AddClearedRows(cleared);

                yield return new WaitForSeconds(clearDelay);
            }
            while (cleared > 0 && !hasWon);

            if (hasWon) yield break;

            hero?.MoveTurn();

            if (monsterManager != null)
                monsterManager.HandleTurn();

            if (CheckGameOver()) yield break;
        }



    }

    private void SpawnShapeFromData()
    {
        currentData = library.PickRandom();
        if (currentData == null) return;

        shapePos = new Vector2Int(
            (columns - currentData.gridSize) / 2,
            rows - currentData.gridSize
        );

        shapeCells = new List<Vector2Int>(currentData.cells.Count);
        shapeBlocks = new List<UIBlock>(currentData.cells.Count);

        foreach (var cd in currentData.cells)
        {
            shapeCells.Add(cd.position);
            var go = Instantiate(blockPrefab, gridContainer);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.sizeDelta = Vector2.one * CellSize;

            var blk = go.GetComponent<UIBlock>();
            blk.Initialize(cd.type);
            shapeBlocks.Add(blk);
        }
        UpdateShapeVisuals();
    }

    private void UpdateShapeVisuals()
    {
        for (int i = 0; i < shapeCells.Count; i++)
        {
            var c = shapeCells[i];
            var rt = shapeBlocks[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(
                (shapePos.x + c.x) * CellSize,
                (shapePos.y + c.y) * CellSize
            );
        }

        // **Refresh the indicator** each time the shape moves or rotates
        if (indicator != null)
        {
            var cols = shapeCells
                .Select(c => shapePos.x + c.x)
                .Distinct();
            indicator.UpdateIndicator(cols);
        }
    }

    private void TryMove(Vector2Int dir)
    {
        var np = shapePos + dir;
        if (CanPlace(shapeCells, np))
        {
            shapePos = np;
            UpdateShapeVisuals();
        }
    }

    private void TryRotate()
    {
        var rot = new List<Vector2Int>();
        foreach (var c in shapeCells)
            rot.Add(new Vector2Int(currentData.gridSize - 1 - c.y, c.x));
        if (CanPlace(rot, shapePos))
        {
            shapeCells = rot;
            UpdateShapeVisuals();
        }
    }

    private void HardDrop()
    {
        int minDrop = int.MaxValue;
        foreach (var c in shapeCells)
        {
            int gx = shapePos.x + c.x, gy = shapePos.y + c.y;
            int drop = gy;
            for (int y = gy - 1; y >= 0; y--)
                if (grid[gx, y] != null)
                {
                    drop = gy - (y + 1);
                    break;
                }
            minDrop = Mathf.Min(minDrop, drop);
        }
        shapePos.y -= minDrop;
        UpdateShapeVisuals();
    }

    private bool CanPlace(List<Vector2Int> cells, Vector2Int pos)
    {
        foreach (var c in cells)
        {
            int x = pos.x + c.x, y = pos.y + c.y;
            if (x < 0 || x >= columns || y < 0) return false;
            if (y < rows && grid[x, y] != null) return false;
        }
        return true;
    }

    private void LockShape()
    {
        for (int i = 0; i < shapeCells.Count; i++)
        {
            var c = shapeCells[i] + shapePos;
            grid[c.x, c.y] = shapeBlocks[i];
        }

        // ★ Ensure the hero is always rendered above the newly-locked blocks
        if (hero != null)
            hero.GetComponent<RectTransform>().SetAsLastSibling();
    }

    private void ApplyGravityToAllColumns()
    {
        for (int x = 0; x < columns; x++)
        {
            int writeY = 0;
            for (int y = 0; y < rows; y++)
            {
                var b = grid[x, y];
                if (b != null)
                {
                    if (y != writeY)
                    {
                        grid[x, writeY] = b;
                        grid[x, y] = null;
                        var rt = b.GetComponent<RectTransform>();
                        rt.anchoredPosition = new Vector2(
                            x * CellSize,
                            writeY * CellSize
                        );
                    }
                    writeY++;
                }
            }
        }
    }

    private int ClearFullRowsOnce()
    {
        int count = 0;
        for (int y = 0; y < rows; y++)
        {
            bool full = true;
            for (int x = 0; x < columns; x++)
                if (grid[x, y] == null) { full = false; break; }
            if (!full) continue;
            for (int x = 0; x < columns; x++)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
            count++;
        }
        return count;
    }

    private bool CheckGameOver()
    {
        for (int x = 0; x < columns; x++)
            if (grid[x, rows - 1] != null)
                return true;
        return false;
    }
}