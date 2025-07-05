// Assets/Scripts/MonsterManager.cs
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [Tooltip("Drag your TetrisController here")]
    public TetrisController tetrisController;

    private MonsterData[] allMonsters;
    private MonsterData[] pool;
    private List<MonsterController> active = new List<MonsterController>();

    void Start()
    {
        // Load all MonsterData assets from Resources/Monsters/
        allMonsters = Resources.LoadAll<MonsterData>("Monsters");
        RefreshPool();
    }

    /// <summary>
    /// Rebuilds the pool of spawnable monsters based on current level difficulty.
    /// </summary>
    public void RefreshPool()
    {
        int lvl = GameManager.Instance.levelDifficulty;
        pool = allMonsters
            .Where(m => m.difficulty <= lvl)
            .ToArray();
    }

    /// <summary>
    /// Called once per turn (after hero.MoveTurn()).
    /// Handles spawning new monsters, applying gravity, and maintaining draw order.
    /// </summary>
    public void HandleTurn()
    {
        var gm = GameManager.Instance;

        // 1) Spawn a new monster if under cap and random check passes
        if (active.Count < gm.MaxMonstersInPlay
            && Random.value < gm.MonsterSpawnChance)
        {
            SpawnMonster();
        }

        // 2) Apply gravity (drop) to all active monsters
        foreach (var m in active)
            m.ApplyGravity();

        // 3) Reorder draw layers: monsters above blocks...
        foreach (var m in active)
        {
            var rt = m.GetComponent<RectTransform>();
            rt.SetAsLastSibling();
        }
        // ...and hero above all monsters
        var heroRT = tetrisController.Hero.GetComponent<RectTransform>();
        heroRT.SetAsLastSibling();
    }

    /// <summary>
    /// Instantiates a new monster container + visual in a random open column.
    /// </summary>
    private void SpawnMonster()
    {
        if (pool.Length == 0) return;

        // Pick a random MonsterData from the pool
        var data = pool[Random.Range(0, pool.Length)];

        // Determine columns not occupied by the hero or another monster
        int heroCol = tetrisController.Hero.CurrentColumn;
        var openCols = Enumerable.Range(0, tetrisController.GridWidth)
            .Where(c => c != heroCol && active.All(m => m.column != c))
            .ToArray();
        if (openCols.Length == 0) return;

        int col = openCols[Random.Range(0, openCols.Length)];

        // Create a new GameObject with RectTransform + MonsterController
        var go = new GameObject($"Monster_{data.monsterId}",
                                typeof(RectTransform),
                                typeof(MonsterController));
        go.transform.SetParent(tetrisController.gridContainer, false);

        // Initialize it
        var mc = go.GetComponent<MonsterController>();
        mc.Initialize(data, col, tetrisController);
        active.Add(mc);
    }

    /// <summary>
    /// Call to remove (and destroy) a monster when it’s killed or leaves play.
    /// </summary>
    public void RemoveMonster(MonsterController mc)
    {
        if (active.Remove(mc))
            Destroy(mc.gameObject);
    }

    /// <summary>
    /// Kills (removes) all active monsters whose column is in the given set.
    /// </summary>
    public void KillMonstersInColumns(IEnumerable<int> columns)
    {
        // Find all monsters matching any of the columns
        var toKill = active
          .Where(m => columns.Contains(m.column))
          .ToList(); // materialize to avoid modifying while iterating

        foreach (var m in toKill)
            RemoveMonster(m);
    }
}
