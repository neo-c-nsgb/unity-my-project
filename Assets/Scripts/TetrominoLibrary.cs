// Assets/Scripts/TetrominoLibrary.cs
using System.Linq;
using UnityEngine;

public class TetrominoLibrary : MonoBehaviour
{
    private TetrominoData[] allShapes;
    private TetrominoData[] pool;

    void Awake()
    {
        allShapes = Resources.LoadAll<TetrominoData>("TetrominoData");
        if (allShapes == null || allShapes.Length == 0)
            Debug.LogError("[TetrominoLibrary] No TetrominoData in Resources/TetrominoData/");
    }

    void Start()
    {
        RefreshPool();
    }

    public void RefreshPool()
    {
        if (allShapes == null) return;
        int lvl = GameManager.Instance != null
            ? GameManager.Instance.levelDifficulty : 0;
        pool = allShapes.Where(t => t.difficulty <= lvl).ToArray();
        if (pool.Length == 0)
        {
            Debug.LogWarning($"[TetrominoLibrary] no shapes ≤ diff {lvl}, using all");
            pool = allShapes;
        }
    }

    public TetrominoData PickRandom()
    {
        if (pool == null || pool.Length == 0) return null;
        return pool[Random.Range(0, pool.Length)];
    }
}