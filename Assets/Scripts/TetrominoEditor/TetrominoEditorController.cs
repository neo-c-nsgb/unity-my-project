// Assets/Scripts/TetrominoEditorController.cs
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TetrominoEditorController : MonoBehaviour
{
    public TMP_InputField shapeNameInput;
    public TMP_InputField difficultyInput;
    public TMP_InputField gridSizeInput;
    public RectTransform contentGrid;      // GridLayoutGroup parent
    public CellButton cellButtonPrefab;
    public Button saveButton;

    List<CellButton> cells = new List<CellButton>();

    void Start()
    {
        gridSizeInput.text = "5";
        BuildGrid(5);

        gridSizeInput.onEndEdit.AddListener(s => {
            if (int.TryParse(s, out int sz) && sz > 0)
                BuildGrid(sz);
        });

        saveButton.onClick.AddListener(SaveAsset);
    }

    void BuildGrid(int gridSize)
    {
        foreach (var cb in cells) Destroy(cb.gameObject);
        cells.Clear();

        var layout = contentGrid.GetComponent<GridLayoutGroup>();
        layout.constraintCount = gridSize;

        for (int y = 0; y < gridSize; y++)
            for (int x = 0; x < gridSize; x++)
            {
                var go = Instantiate(cellButtonPrefab.gameObject, contentGrid);
                var cb = go.GetComponent<CellButton>();
                cb.x = x; cb.y = y;
                cb.currentType = BlockType.None;
                cb.Refresh();
                cells.Add(cb);
            }
    }

    void SaveAsset()
    {
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        data.shapeName = shapeNameInput.text;
        data.gridSize = int.Parse(gridSizeInput.text);
        data.difficulty = int.Parse(difficultyInput.text);
        data.cells = new List<CellData>();

        foreach (var cb in cells)
            if (cb.currentType != BlockType.None)
                data.cells.Add(new CellData
                {
                    position = new Vector2Int(cb.x, cb.y),
                    type = cb.currentType
                });

#if UNITY_EDITOR
        const string folder = "Assets/TetrominoData";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "TetrominoData");
        string path = $"{folder}/{data.shapeName}.asset";
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = data;
#endif
    }
}
