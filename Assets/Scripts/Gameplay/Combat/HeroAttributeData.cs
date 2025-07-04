// Assets/Scripts/HeroAttributeData.cs
using UnityEngine;


[CreateAssetMenu(fileName = "HeroAttributes_", menuName = "Heroes/Hero Attribute Data")]
public class HeroAttributeData : ScriptableObject
{
    [Header("Identity")]
    public string heroId;
    public string heroName;
    public Sprite portrait;

    [Header("Combat")]
    public int maxHP;
    public DamageType damageType;
    public int baseDamage;
    [Range(0, 1)] public float critChance;
    [Tooltip("+% damage on crit")] public float critDamageBonus;
    [Range(0, 1)] public float dodgeChance;
    public int armor;

    [Header("Resources")]
    public int maxEnergy;
    [Range(0, 1)] public float energyRegenPerTurn;

    [Header("Movement")]
    public int moveSpeed;         // columns per turn
    public int heightClimbLimit;  // max height diff

    [Header("Prefab (Resources Path)")]
    [Tooltip("Path under Resources to the hero prefab (e.g. \"Prefabs/HeroKnight\"). No extension.")]
    public string heroPrefabPath;

    /// <summary>
    /// Loads the hero prefab from Resources at runtime.
    /// </summary>
    public GameObject Prefab
    {
        get
        {
            if (string.IsNullOrEmpty(heroPrefabPath))
                return null;
            return Resources.Load<GameObject>(heroPrefabPath);
        }
    }
}
