// Assets/Scripts/ItemData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData_", menuName = "GameData/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string itemName;

    [Header("Bonuses")]
    public int hpBonus;
    public int damageBonus;
    public int armorBonus;
    [Range(0f, 1f)] public float dodgeChance;
    [Range(0f, 1f)] public float critChance;
    [Tooltip("+% damage on crit")]
    public float critDamageBonus;

    [Header("Assets (Resources Paths)")]
    [Tooltip("Path under Resources to the item prefab (e.g. \"Prefabs/Sword\"). No file extension.")]
    public string prefabPath;
    [Tooltip("Path under Resources to the icon sprite (e.g. \"Sprites/SwordIcon\"). No file extension.")]
    public string iconPath;

    [Header("Shape (Grid Editor)")]
    [Tooltip("Size of the square grid")]
    public int gridSize = 5;
    [Tooltip("Occupied cells within 0..gridSize-1")]
    public List<Vector2Int> cells = new List<Vector2Int>();

    /// <summary>
    /// Loads the prefab from Resources using prefabPath.
    /// </summary>
    public GameObject Prefab => string.IsNullOrEmpty(prefabPath)
        ? null
        : Resources.Load<GameObject>(prefabPath);

    /// <summary>
    /// Loads the icon sprite from Resources using iconPath.
    /// </summary>
    public Sprite Icon => string.IsNullOrEmpty(iconPath)
        ? null
        : Resources.Load<Sprite>(iconPath);
}
