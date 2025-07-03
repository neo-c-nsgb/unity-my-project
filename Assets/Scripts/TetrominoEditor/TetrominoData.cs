// Assets/Scripts/TetrominoData.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CellData
{
    public Vector2Int position;
    public BlockType type;
}

[CreateAssetMenu(fileName = "Tetromino", menuName = "Tetris/Tetromino Data")]
public class TetrominoData : ScriptableObject
{
    public string shapeName;
    [Min(1)] public int gridSize = 5;
    [Min(0)] public int difficulty;
    public List<CellData> cells = new List<CellData>();
}
