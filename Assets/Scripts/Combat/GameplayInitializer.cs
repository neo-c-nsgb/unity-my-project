// Assets/Scripts/GameplayInitializer.cs
using System.Collections;
using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
    [Tooltip("Drag your TetrisController here")]
    public TetrisController tetrisController;

    void Start()
    {
        // Kick off the init in a coroutine so TetrisController.Start() runs first
        StartCoroutine(InitNextFrame());
    }

    private IEnumerator InitNextFrame()
    {
        // wait one frame
        yield return null;

        // 1) Validate the controller
        if (tetrisController == null)
        {
            Debug.LogError("GameplayInitializer: TetrisController is null!");
            yield break;
        }

        // 2) Make sure grid dimensions are valid
        int gridW = tetrisController.GridWidth;
        int gridH = tetrisController.GridHeight;
        if (gridW <= 0 || gridH <= 0)
        {
            Debug.LogError(
                $"GameplayInitializer: Invalid grid size {gridW}×{gridH}. " +
                "Ensure TetrisController sets GridWidth/GridHeight in Start()."
            );
            yield break;
        }

        // 3) Get the selected hero config
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

        // 4) Check the prefab
        if (cfg.heroPrefab == null)
        {
            Debug.LogError(
                $"GameplayInitializer: Hero '{cfg.heroId}' has no heroPrefab assigned!"
            );
            yield break;
        }

        // 5) Instantiate under the grid container
        var heroGO = Instantiate(cfg.heroPrefab, tetrisController.gridContainer, false);

        // 6) Force bottom-left anchoring/pivot
        var heroRT = heroGO.GetComponent<RectTransform>();
        heroRT.anchorMin = heroRT.anchorMax = Vector2.zero;
        heroRT.pivot = Vector2.zero;

        // 7) Apply stats
        var heroCtrl = heroGO.GetComponent<HeroController>();
        if (heroCtrl == null)
        {
            Debug.LogError("GameplayInitializer: Prefab lacks HeroController!");
            yield break;
        }

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
        heroCtrl.energyRegenPerTurn = cfg.energyRegenPerTurn;
        heroCtrl.moveDistance = cfg.moveSpeed;
        heroCtrl.heightDiffLimit = cfg.heightClimbLimit;

        // 8) Register with TetrisController
        tetrisController.RegisterHero(heroCtrl);
        heroCtrl.tetrisController = tetrisController;

        // 9) Center in middle column
        int centerCol = Mathf.FloorToInt(gridW / 2f);
        heroCtrl.SetColumn(centerCol);
        heroCtrl.UpdatePosition();

        Debug.Log($"GameplayInitializer: Hero placed at column {centerCol} of {gridW}.");
    }
}
