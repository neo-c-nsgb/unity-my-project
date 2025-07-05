// Assets/Scripts/MonsterData.cs
using UnityEngine;



[CreateAssetMenu(fileName = "MonsterData_", menuName = "GameData/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Identity")]
    public string monsterId;
    public string monsterName;

    [Header("Gameplay")]
    [Tooltip("Tier of difficulty (≤ levelDifficulty)")]
    public int difficulty;
    [Tooltip("Max simultaneous monsters at this difficulty")]
    public int maxSimultaneous;

    [Header("Stats")]
    public int maxHP;
    public int damage;
    public DamageType damageType;
    [Range(0f, 1f)] public float dodgeChance;
    [Range(0f, 1f)] public float critChance;
    [Tooltip("+% damage on crit")]
    public float critDamageBonus;

    [Header("Visual Prefab (Resources Path)")]
    [Tooltip("Path under Resources to the monster’s full prefab (e.g. \"Prefabs/Monsters/Goblin\"). No extension.")]
    public string monsterPrefabPath;



    /// <summary>
    /// Loads the monster’s visual prefab via Resources.Load.
    /// </summary>
    public GameObject Prefab
    {
        get
        {
            if (string.IsNullOrEmpty(monsterPrefabPath)) return null;
            var go = Resources.Load<GameObject>(monsterPrefabPath);
            if (go == null)
                Debug.LogError($"MonsterData '{monsterId}': prefab not found at Resources/{monsterPrefabPath}");
            return go;
        }
    }
}
