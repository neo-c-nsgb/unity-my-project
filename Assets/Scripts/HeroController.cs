using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroController : MonoBehaviour
{

    /// <summary>Which column the hero is currently on (0..GridWidth-1).</summary>
    public int CurrentColumn => currentColumn;

    [Tooltip("Reference to your TetrisController")]
    public TetrisController tetrisController;

    [Tooltip("How many columns to move per turn")]
    public int moveDistance = 1;

    [Tooltip("Max allowed height difference between adjacent columns")]
    public int heightDiffLimit = 2;

    [Tooltip("Starting column index (0 = leftmost)")]
    public int startColumn = 0;

    [Tooltip("Start moving right if true, left if false")]
    public bool startMovingRight = true;

    // internal state
    private int currentColumn;
    private bool movingRight;

    /// <summary> True if hero is currently moving/facing right </summary>
    public bool FacingRight => movingRight;

    void Start()
    {
        if (tetrisController == null)
        {
            Debug.LogError("HeroController requires a TetrisController reference!");
            enabled = false;
            return;
        }

        currentColumn = Mathf.Clamp(startColumn, 0, tetrisController.GridWidth - 1);
        movingRight = startMovingRight;
        UpdatePosition();
    }

    /// <summary>
    /// Call this once per turn, after all clears+gravity are done.
    /// </summary>
    public void MoveTurn()
    {
        int remaining = moveDistance;
        while (remaining > 0)
        {
            int next = currentColumn + (movingRight ? 1 : -1);

            // hit wall → reverse direction, retry same step
            if (next < 0 || next >= tetrisController.GridWidth)
            {
                movingRight = !movingRight;
                continue;
            }

            // check height difference
            int hCurr = tetrisController.GetColumnHeight(currentColumn);
            int hNext = tetrisController.GetColumnHeight(next);
            if (Mathf.Abs(hNext - hCurr) > heightDiffLimit)
            {
                // blocked → reverse and retry
                movingRight = !movingRight;
                continue;
            }

            // valid move
            currentColumn = next;
            remaining--;
        }

        UpdatePosition();
    }

    public void UpdatePosition()
    {
        int h = tetrisController.GetColumnHeight(currentColumn);
        float x = currentColumn * tetrisController.CellSize;
        float y = h * tetrisController.CellSize;

        var rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
    }
}
