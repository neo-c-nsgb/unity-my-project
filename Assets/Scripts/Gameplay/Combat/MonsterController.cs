// Assets/Scripts/Gameplay/Combat/MonsterController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class MonsterController : MonoBehaviour
{
    public MonsterData data { get; private set; }
    public int column { get; private set; }
    public int currentHP { get; private set; }

    private TetrisController tetris;
    private RectTransform rt;

    [Header("UI (wire these in your MonsterUIPrefab)")]
    public TMP_Text damageText;      // e.g. "20"
    public Image damageTypeIcon;  // melee/ranged
    public Image hpBarImage;      // horizontal fillbar
    public GameObject hpBarContainer;// parent of hpBarImage

    // global icons
    private Sprite meleeIcon;
    private Sprite rangedIcon;
    const string MeleeIconPath = "UI/Icons/MeleeIcon";
    const string RangedIconPath = "UI/Icons/RangedIcon";

    void Awake()
    {
        // preload your two damage-type icons
        meleeIcon = Resources.Load<Sprite>(MeleeIconPath);
        rangedIcon = Resources.Load<Sprite>(RangedIconPath);
    }

    /// <summary>
    /// Must be called immediately after Instantiate() by MonsterManager.
    /// </summary>
    public void Initialize(MonsterData data, int col, TetrisController tetrisController)
    {
        this.data = data;
        this.column = col;
        this.tetris = tetrisController;
        this.rt = GetComponent<RectTransform>();

        float cell = tetris.CellSize;

        // — 1) Position the container —
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.anchoredPosition = new Vector2(col * cell, tetris.GridHeight * cell);
        transform.SetParent(tetris.gridContainer, false);

        // — 2) Spawn the “visual” prefab under this container —
        var visPrefab = data.Prefab;
        if (visPrefab == null)
        {
            Debug.LogError(
                $"[MonsterController] Couldn’t load visual prefab for '{data.monsterId}'.\n" +
                $"  monsterPrefabPath = '{data.monsterPrefabPath}'\n" +
                $"  Did you put the prefab under Resources/{data.monsterPrefabPath}.prefab?"
            );
        }
        else
        {
            var go = Instantiate(visPrefab, transform, false);
            var vRT = go.GetComponent<RectTransform>();
            vRT.anchorMin = vRT.anchorMax = Vector2.zero;
            vRT.pivot = new Vector2(0.5f, 0f);
            vRT.sizeDelta = new Vector2(cell, cell);
            vRT.anchoredPosition = new Vector2(cell * 0.5f, cell * 0.5f);

            // ensure this visual sits _behind_ your stats UI:
            go.transform.SetAsFirstSibling();
        }

        // — 3) Initialize HP + damage UI —
        currentHP = data.maxHP;
        damageText.text = data.damage.ToString();
        if (damageTypeIcon != null)
        {
            damageTypeIcon.sprite = (data.damageType == DamageType.Melee)
                ? meleeIcon
                : rangedIcon;
        }

        UpdateHPBar();
    }

    /// <summary>Apply damage, refresh the HP bar, and remove if dead.</summary>
    public void ApplyDamage(int amount)
    {
        currentHP = Mathf.Clamp(currentHP - amount, 0, data.maxHP);
        UpdateHPBar();
        if (currentHP == 0)
            tetris.monsterManager.RemoveMonster(this);
    }

    private void UpdateHPBar()
    {
        if (hpBarImage != null)
            hpBarImage.fillAmount = (float)currentHP / data.maxHP;

        if (hpBarContainer != null)
        {
            bool visible = currentHP > 0 && currentHP < data.maxHP;
            hpBarContainer.SetActive(visible);
        }
    }

    /// <summary>Called each turn to drop this monster onto the stack.</summary>
    public void ApplyGravity()
    {
        float cell = tetris.CellSize;
        int height = tetris.GetColumnHeight(column);
        rt.anchoredPosition = new Vector2(column * cell, height * cell);
    }

    void LateUpdate()
    {
        // Optionally flip any visual you spawned here
    }
}
