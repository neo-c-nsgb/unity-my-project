// Assets/Scripts/HeroController.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class HeroController : MonoBehaviour
{
    [Tooltip("Reference to your TetrisController")]
    public TetrisController tetrisController;

    [Tooltip("How many columns to move per turn")]
    public int moveDistance = 1;

    [Tooltip("Max allowed height difference between adjacent columns")]
    public int heightDiffLimit = 2;

    [Tooltip("Fallback starting column if not set externally")]
    public int startColumn = 0;

    [Tooltip("Start moving right if true; left if false")]
    public bool startMovingRight = true;

    [Header("Stats")]
    public int maxHP;
    [HideInInspector] public int currentHP;
    public DamageType damageType;
    public int baseDamage;
    [Range(0f, 1f)] public float critChance;
    public float critDamageBonus;
    [Range(0f, 1f)] public float dodgeChance;
    public int armor;

    [Header("Resource")]
    public int maxEnergy;
    [HideInInspector] public float currentEnergy;
    [Tooltip("Energy regenerated each turn")]
    public float energyRegenPerTurn;

    [Header("Events")]
    [Tooltip("Fired when the hero is blocked on both sides and can't move")]
    public UnityEvent OnBlocked;

    // Internal state
    private int currentColumn;
    private bool movingRight;
    private bool columnSetExternally = false;

    /// <summary>Current column index (0 = leftmost).</summary>
    public int CurrentColumn => currentColumn;
    /// <summary>True if hero is facing right.</summary>
    public bool FacingRight => movingRight;

    void Start()
    {
        if (tetrisController == null)
        {
            Debug.LogError("HeroController requires a TetrisController reference!");
            enabled = false;
            return;
        }

        // only use fallback startColumn if nothing set externally
        if (!columnSetExternally)
            currentColumn = Mathf.Clamp(startColumn, 0, tetrisController.GridWidth - 1);

        movingRight = startMovingRight;
        currentHP = maxHP;
        currentEnergy = maxEnergy;

        UpdatePosition();
    }

    /// <summary>Force the hero into a specific column immediately.</summary>
    public void SetColumn(int col)
    {
        currentColumn = Mathf.Clamp(col, 0, tetrisController.GridWidth - 1);
        columnSetExternally = true;
    }

    /// <summary>
    /// Moves the hero each turn, but avoids infinite loops if blocked both ways.
    /// </summary>
    public void MoveTurn()
    {
        int steps = moveDistance;
        for (int step = 0; step < steps; step++)
        {
            bool moved = false;
            for (int attempt = 0; attempt < 2; attempt++)
            {
                int next = currentColumn + (movingRight ? 1 : -1);

                if (next >= 0 && next < tetrisController.GridWidth)
                {
                    int hCurr = tetrisController.GetColumnHeight(currentColumn);
                    int hNext = tetrisController.GetColumnHeight(next);
                    if (Mathf.Abs(hNext - hCurr) <= heightDiffLimit)
                    {
                        currentColumn = next;
                        moved = true;
                        break;
                    }
                }
                movingRight = !movingRight;
            }

            if (!moved)
            {
                Debug.LogWarning(
                    $"Hero blocked at column {currentColumn}: both sides too steep or edges."
                );
                OnBlocked?.Invoke();
                break;  // bail out so we don't loop endlessly
            }
        }

        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRegenPerTurn);
        UpdatePosition();
    }

    /// <summary>Positions the hero RectTransform atop its column stack.</summary>
    public void UpdatePosition()
    {
        // clamp column just in case
        currentColumn = Mathf.Clamp(currentColumn, 0, tetrisController.GridWidth - 1);
        int height = tetrisController.GetColumnHeight(currentColumn);
        float x = currentColumn * tetrisController.CellSize;
        float y = height * tetrisController.CellSize;

        var rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
    }
}
