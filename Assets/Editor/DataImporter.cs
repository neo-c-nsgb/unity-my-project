// Assets/Editor/DataImporter.cs
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class DataImporter
{
    [MenuItem("Tools/Import Monsters from CSV")]
    public static void ImportMonsters()
    {
        string csvPath = "Assets/Resources/Data/Monsters.csv";
        if (!File.Exists(csvPath))
        {
            Debug.LogError("Monsters.csv not found at " + csvPath);
            return;
        }
        var lines = File.ReadAllLines(csvPath);
        if (lines.Length < 2)
        {
            Debug.LogError("Monsters.csv contains no data rows!");
            return;
        }

        string outFolder = "Assets/Resources/Monsters";
        if (!AssetDatabase.IsValidFolder(outFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "Monsters");

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            var cols = line.Split(',');

            // Expected columns order:
            // 0: monsterId
            // 1: monsterName
            // 2: maxHP
            // 3: damage
            // 4: damageType
            // 5: dodgeChance
            // 6: critChance
            // 7: critDamageBonus
            // 8: prefabPath
            // 9: difficulty 

            int last = cols.Length - 1;

            string id = cols[0].Trim();
            string name = cols[1].Trim();
            int hp = int.Parse(cols[2]);
            int dmg = int.Parse(cols[3]);
            var dt = (DamageType)Enum.Parse(typeof(DamageType), cols[4].Trim(), true);
            float dodge = float.Parse(cols[5]);
            float cc = float.Parse(cols[6]);
            float cd = float.Parse(cols[7]);
            string pfPath = cols[8].Trim();
            int diff = int.Parse(cols[last]);

            var prefab = Resources.Load<GameObject>(pfPath);
            if (prefab == null)
                Debug.LogWarning($"Monster '{id}': prefab '{pfPath}' not found in Resources.");

            string assetPath = $"{outFolder}/{id}.asset";
            var data = AssetDatabase.LoadAssetAtPath<MonsterData>(assetPath)
                       ?? ScriptableObject.CreateInstance<MonsterData>();

            data.monsterId = id;
            data.monsterName = name;
            data.maxHP = hp;
            data.damage = dmg;
            data.damageType = dt;
            data.dodgeChance = dodge;
            data.critChance = cc;
            data.critDamageBonus = cd;
            data.monsterPrefab = prefab;
            data.difficulty = diff;

            if (!AssetDatabase.Contains(data))
                AssetDatabase.CreateAsset(data, assetPath);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Imported {lines.Length - 1} monsters.");
    }

    [MenuItem("Tools/Import Items from CSV")]
    public static void ImportItems()
    {
        string csvPath = "Assets/Resources/Data/Items.csv";
        if (!File.Exists(csvPath))
        {
            Debug.LogError("Items.csv not found at " + csvPath);
            return;
        }
        var lines = File.ReadAllLines(csvPath);
        if (lines.Length < 2)
        {
            Debug.LogError("Items.csv contains no data rows!");
            return;
        }

        string outFolder = "Assets/Resources/Items";
        if (!AssetDatabase.IsValidFolder(outFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "Items");

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            var cols = line.Split(',');

            // Expected columns order:
            // 0: itemId
            // 1: itemName
            // 2: hpBonus
            // 3: damageBonus
            // 4: armorBonus
            // 5: dodgeChance
            // 6: critChance
            // 7: critDamageBonus
            // 8: prefabPath
            // 9: spritePath
            // 10: shapeName  

            int last = cols.Length - 1;

            string id = cols[0].Trim();
            string name = cols[1].Trim();
            int hpB = int.Parse(cols[2]);
            int dmgB = int.Parse(cols[3]);
            int armB = int.Parse(cols[4]);
            float dodge = float.Parse(cols[5]);
            float cc = float.Parse(cols[6]);
            float cd = float.Parse(cols[7]);
            string pfPath = cols[8].Trim();
            string spriteKey = cols[9].Trim();
            string shape = cols[last].Trim();

            var prefab = Resources.Load<GameObject>(pfPath);
            if (prefab == null)
                Debug.LogWarning($"Item '{id}': prefab '{pfPath}' not found.");
            var icon = Resources.Load<Sprite>(spriteKey);
            if (icon == null)
                Debug.LogWarning($"Item '{id}': sprite '{spriteKey}' not found.");

            string assetPath = $"{outFolder}/{id}.asset";
            var data = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath)
                       ?? ScriptableObject.CreateInstance<ItemData>();

            data.itemId = id;
            data.itemName = name;
            data.hpBonus = hpB;
            data.damageBonus = dmgB;
            data.armorBonus = armB;
            data.dodgeChance = dodge;
            data.critChance = cc;
            data.critDamageBonus = cd;
            data.itemPrefab = prefab;
            data.icon = icon;
            data.shapeName = shape;

            if (!AssetDatabase.Contains(data))
                AssetDatabase.CreateAsset(data, assetPath);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Imported {lines.Length - 1} items.");
    }
}
