// Assets/Scripts/ItemData.cs
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

    [Header("Assets")]
    public GameObject itemPrefab;
    public Sprite icon;

    [Header("Visual")]
    [Tooltip("Logical shape name for grid/equipment UI (e.g. \"T\", \"I\", \"Square\")")]
    public string shapeName;
}
