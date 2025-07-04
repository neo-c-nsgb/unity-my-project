// Assets/Scripts/ItemEditor.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemEditor : MonoBehaviour
{
    [Header("Data Selector")]
    public TMP_Dropdown itemDropdown;

    [Header("Stats (TextMeshPro Input)")]
    public TMP_InputField idInput;
    public TMP_InputField nameInput;
    public TMP_InputField hpInput;
    public TMP_InputField dmgInput;
    public TMP_InputField armorInput;
    public TMP_InputField dodgeInput;
    public TMP_InputField critChInput;
    public TMP_InputField critDbInput;

    [Header("Assets Paths (TextMeshPro Input)")]
    [Tooltip("e.g. Prefabs/Sword (no extension)")]
    public TMP_InputField prefabPathInput;
    [Tooltip("e.g. Sprites/SwordIcon (no extension)")]
    public TMP_InputField iconPathInput;

    [Header("Shape Grid Size")]
    public TMP_InputField gridSizeInput;

    [Header("Shape Cells")]
    public RectTransform shapeGridContent;
    public Toggle cellTogglePrefab;

    [Header("Save")]
    public Button saveButton;

    // internal
    ItemData[] allItems;
    ItemData currentItem;
    List<Toggle> cellToggles = new List<Toggle>();

    void Start()
    {
        // load all ItemData assets
        allItems = Resources.LoadAll<ItemData>("Items");
        var names = new List<string>();
        foreach (var it in allItems) names.Add(it.itemName);
        itemDropdown.ClearOptions();
        itemDropdown.AddOptions(names);

        itemDropdown.onValueChanged.AddListener(idx => LoadItem(idx));
        saveButton.onClick.AddListener(SaveCurrentItem);

        if (allItems.Length > 0)
            LoadItem(0);
    }

    void LoadItem(int idx)
    {
        currentItem = allItems[idx];

        // populate fields
        idInput.text = currentItem.itemId;
        nameInput.text = currentItem.itemName;
        hpInput.text = currentItem.hpBonus.ToString();
        dmgInput.text = currentItem.damageBonus.ToString();
        armorInput.text = currentItem.armorBonus.ToString();
        dodgeInput.text = currentItem.dodgeChance.ToString();
        critChInput.text = currentItem.critChance.ToString();
        critDbInput.text = currentItem.critDamageBonus.ToString();

        prefabPathInput.text = currentItem.prefabPath;
        iconPathInput.text = currentItem.iconPath;

        gridSizeInput.text = currentItem.gridSize.ToString();
        RebuildShapeGrid();
    }

    void RebuildShapeGrid()
    {
        // clear old toggles
        foreach (var t in cellToggles) Destroy(t.gameObject);
        cellToggles.Clear();

        // parse grid size
        if (!int.TryParse(gridSizeInput.text, out int sz) || sz < 1) sz = 1;
        currentItem.gridSize = sz;

        // configure layout
        var layout = shapeGridContent.GetComponent<GridLayoutGroup>();
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = sz;

        // build toggles
        for (int y = sz - 1; y >= 0; y--)
        {
            for (int x = 0; x < sz; x++)
            {
                var t = Instantiate(cellTogglePrefab, shapeGridContent);
                var coord = new Vector2Int(x, y);
                bool on = currentItem.cells.Contains(coord);
                t.isOn = on;

                t.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        if (!currentItem.cells.Contains(coord))
                            currentItem.cells.Add(coord);
                    }
                    else
                    {
                        currentItem.cells.RemoveAll(c => c == coord);
                    }
                });

                cellToggles.Add(t);
            }
        }
    }

    void SaveCurrentItem()
    {
        if (currentItem == null) return;

        // write back stats
        currentItem.itemId = idInput.text;
        currentItem.itemName = nameInput.text;
        int.TryParse(hpInput.text, out currentItem.hpBonus);
        int.TryParse(dmgInput.text, out currentItem.damageBonus);
        int.TryParse(armorInput.text, out currentItem.armorBonus);
        float.TryParse(dodgeInput.text, out currentItem.dodgeChance);
        float.TryParse(critChInput.text, out currentItem.critChance);
        float.TryParse(critDbInput.text, out currentItem.critDamageBonus);

        // write back resource paths
        currentItem.prefabPath = prefabPathInput.text;
        currentItem.iconPath = iconPathInput.text;

#if UNITY_EDITOR
        EditorUtility.SetDirty(currentItem);
        AssetDatabase.SaveAssets();
        Debug.Log($"Saved ItemData '{currentItem.itemName}'");
#endif
    }
}
