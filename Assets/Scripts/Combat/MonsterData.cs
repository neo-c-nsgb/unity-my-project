// Assets/Scripts/MonsterData.cs
using UnityEngine;


[CreateAssetMenu(fileName = "MonsterData_", menuName = "GameData/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Identity")]
    public string monsterId;
    public string monsterName;

    [Header("Stats")]
    public int maxHP;
    public int damage;
    public DamageType damageType;
    [Range(0f, 1f)] public float dodgeChance;
    [Range(0f, 1f)] public float critChance;
    [Tooltip("+% damage on crit")]
    public float critDamageBonus;

    [Header("Prefab")]
    public GameObject monsterPrefab;

    [Header("Gameplay")]
    [Tooltip("Tier of difficulty for random spawn/filter (higher = tougher)")]
    public int difficulty;
}
