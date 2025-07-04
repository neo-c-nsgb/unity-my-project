// Assets/Scripts/GameplayInitializer.cs
using System.Collections;
using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
    [Tooltip("Drag your TetrisController here")]
    public TetrisController tetrisController;

    void Start()
    {
        // Run on next frame so TetrisController has set up its grid
        StartCoroutine(InitNextFrame());
    }

    private IEnumerator InitNextFrame()
    {
        yield return null;

        if (tetrisController == null)
        {
            Debug.LogError("GameplayInitializer: TetrisController is null!");
            yield break;
        }

        int gridW = tetrisController.GridWidth;
        int gridH = tetrisController.GridHeight;
        if (gridW <= 0 || gridH <= 0)
        {
            Debug.LogError($"GameplayInitializer: Invalid grid {gridW}×{gridH}");
            yield break;
        }

        var pdm = PlayerDataManager.Instance;
        if (pdm == null)
        {
            Debug.LogError("GameplayInitializer: No PlayerDataManager found!");
            yield break;
        }

        var cfg = pdm.GetSelectedHeroConfig();
        if (cfg == null)
        {
            Debug.LogError("GameplayInitializer: No hero selected or config missing!");
            yield break;
        }

        // **Use the new Prefab property**
        var prefab = cfg.Prefab;
        if (prefab == null)
        {
            Debug.LogError(
                $"GameplayInitializer: Hero '{cfg.heroId}' prefab not found.\n" +
                $"Checked Resources at path '{cfg.heroPrefabPath}'."
            );
            yield break;
        }

        // Instantiate under the grid container
        var heroGO = Instantiate(prefab, tetrisController.gridContainer, false);

        // Force bottom-left anchoring/pivot
        var heroRT = heroGO.GetComponent<RectTransform>();
        heroRT.anchorMin = heroRT.anchorMax = Vector2.zero;
        heroRT.pivot = Vector2.zero;

        // Fetch and configure the controller
        var heroCtrl = heroGO.GetComponent<HeroController>();
        if (heroCtrl == null)
        {
            Debug.LogError("GameplayInitializer: Prefab lacks HeroController!");
            yield break;
        }

        // Apply stats from HeroAttributeData
        heroCtrl.maxHP = cfg.maxHP;
        heroCtrl.currentHP = cfg.maxHP;
        heroCtrl.damageType = cfg.damageType;
        heroCtrl.baseDamage = cfg.baseDamage;
        heroCtrl.critChance = cfg.critChance;
        heroCtrl.critDamageBonus = cfg.critDamageBonus;
        heroCtrl.dodgeChance = cfg.dodgeChance;
        heroCtrl.armor = cfg.armor;
        heroCtrl.maxEnergy = cfg.maxEnergy;
        heroCtrl.currentEnergy = cfg.maxEnergy;
        heroCtrl.energyRegenPerTurn = (int)cfg.energyRegenPerTurn;
        heroCtrl.moveDistance = cfg.moveSpeed;
        heroCtrl.heightDiffLimit = cfg.heightClimbLimit;

        // Register with TetrisController
        tetrisController.RegisterHero(heroCtrl);
        heroCtrl.tetrisController = tetrisController;

        // Center in middle column
        int centerCol = Mathf.FloorToInt(gridW / 2f);
        heroCtrl.SetColumn(centerCol);
        heroCtrl.UpdatePosition();

        Debug.Log($"GameplayInitializer: Hero placed at column {centerCol} of {gridW}.");

        var statsUI = Object.FindFirstObjectByType<HeroStatsUI>();
        if (statsUI != null)
            statsUI.hero = heroCtrl;
    }
}
