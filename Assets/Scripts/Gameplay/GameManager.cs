// Assets/Scripts/GameManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    [Tooltip("Tier ID (e.g. 1,2,3…)")]
    public int difficulty;
    [Tooltip("Columns for this tier")] public int gridWidth;
    [Tooltip("Rows for this tier")] public int gridHeight;
    [Tooltip("Rows to clear to win")] public int winRowCount;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [Tooltip("Select which config-tier to use")]
    public int levelDifficulty = 1;
    [Tooltip("Define parameters per tier")]
    public List<LevelConfig> levelConfigs;

    private LevelConfig currentConfig;
    private int rowsCleared;
    public event Action OnWin;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentConfig = levelConfigs.Find(c => c.difficulty == levelDifficulty);
        if (currentConfig == null)
        {
            Debug.LogError($"GameManager: no config for tier {levelDifficulty}");
            enabled = false;
        }
    }

    public int GridWidth => currentConfig.gridWidth;
    public int GridHeight => currentConfig.gridHeight;
    public int WinRowCount => currentConfig.winRowCount;
    public int RowsCleared => rowsCleared;

    public void AddClearedRows(int count)
    {
        if (count <= 0) return;
        rowsCleared += count;
        Debug.Log($"Rows cleared: {rowsCleared}/{WinRowCount}");
        if (rowsCleared >= WinRowCount)
        {
            Debug.Log("🏆 You win!");
            OnWin?.Invoke();
        }
    }
}