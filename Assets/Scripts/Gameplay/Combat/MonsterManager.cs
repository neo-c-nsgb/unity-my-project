// Assets/Scripts/Gameplay/Combat/MonsterManager.cs
using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    [Tooltip("Drag your TetrisController here")]
    public TetrisController tetrisController;

    [Tooltip("Your fully‐setup MonsterUI prefab")]
    public GameObject monsterUIPrefab;

    private MonsterData[] allMonsters;
    private MonsterData[] pool;
    private List<MonsterController> active = new List<MonsterController>();

    void Start()
    {
        // Load & filter based on level
        allMonsters = Resources.LoadAll<MonsterData>("Monsters");
        RefreshPool();
    }

    public void RefreshPool()
    {
        List<MonsterData> temp = new List<MonsterData>();
        int lvl = GameManager.Instance.levelDifficulty;
        foreach (var m in allMonsters)
            if (m.difficulty <= lvl)
                temp.Add(m);
        pool = temp.ToArray();
    }

    /// <summary>
    /// Call each turn (after hero.MoveTurn()).
    /// Spawns, drops, and re-layers monsters.
    /// </summary>
    public void HandleTurn()
    {
        var gm = GameManager.Instance;

        // Maybe spawn
        if (active.Count < gm.MaxMonstersInPlay
            && Random.value < gm.MonsterSpawnChance)
        {
            SpawnMonster();
        }

        // Gravity
        foreach (var m in active)
            m.ApplyGravity();

        // Draw order: monsters above blocks
        foreach (var m in active)
            m.GetComponent<RectTransform>().SetAsLastSibling();
        // Hero on top
        tetrisController.Hero
            .GetComponent<RectTransform>()
            .SetAsLastSibling();
    }

    private void SpawnMonster()
    {
        if (pool == null || pool.Length == 0) return;

        // Pick random data
        int idx = Random.Range(0, pool.Length);
        MonsterData data = pool[idx];

        // Find open columns
        int heroCol = tetrisController.Hero.CurrentColumn;
        List<int> openCols = new List<int>();
        for (int c = 0; c < tetrisController.GridWidth; c++)
        {
            if (c == heroCol) continue;
            bool occupied = false;
            foreach (var m in active)
                if (m.column == c) { occupied = true; break; }
            if (!occupied) openCols.Add(c);
        }
        if (openCols.Count == 0) return;

        int col = openCols[Random.Range(0, openCols.Count)];

        // Instantiate your prefab (with all UI children already wired)
        var go = Instantiate(monsterUIPrefab,
                             tetrisController.gridContainer,
                             false);
        var mc = go.GetComponent<MonsterController>();
        mc.Initialize(data, col, tetrisController);
        active.Add(mc);
    }

    /// <summary>
    /// Instantly kills monsters in the dropped columns.
    /// </summary>
    public void KillMonstersInColumns(int[] columns)
    {
        List<MonsterController> toKill = new List<MonsterController>();
        foreach (var m in active)
        {
            foreach (var c in columns)
                if (m.column == c)
                {
                    toKill.Add(m);
                    break;
                }
        }
        foreach (var m in toKill)
            RemoveMonster(m);
    }

    public void RemoveMonster(MonsterController mc)
    {
        if (active.Remove(mc))
            Destroy(mc.gameObject);
    }
}
