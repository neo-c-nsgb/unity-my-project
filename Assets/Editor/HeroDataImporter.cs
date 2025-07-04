// Assets/Editor/HeroDataImporter.cs
using System.IO;
using UnityEditor;
using UnityEngine;

public static class HeroDataImporter
{
    [MenuItem("Tools/Import Heroes from CSV")]
    public static void ImportFromCSV()
    {
        const string csvPath = "Assets/Resources/Data/Heroes.csv";
        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSV not found at " + csvPath);
            return;
        }

        var lines = File.ReadAllLines(csvPath);
        if (lines.Length < 2)
        {
            Debug.LogError("Heroes.csv has no data rows!");
            return;
        }

        const string outFolder = "Assets/Resources/Heroes";
        if (!AssetDatabase.IsValidFolder(outFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "Heroes");

        // assume header defines columns; prefabPath is the last column
        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Trim();
            if (string.IsNullOrEmpty(row)) continue;
            var cols = row.Split(',');

            // Expected columns:
            // 0: heroId
            // 1: heroName
            // 2: portraitPath
            // 3: maxHP
            // 4: damageType
            // 5: baseDamage
            // 6: critChance
            // 7: critDamageBonus
            // 8: dodgeChance
            // 9: armor
            // 10: maxEnergy
            // 11: energyRegenPerTurn
            // 12: moveSpeed
            // 13: heightClimbLimit
            // 14: heroPrefabPath 

            int last = cols.Length - 1;
            string id = cols[0].Trim();
            string name = cols[1].Trim();
            string portrait = cols[2].Trim();
            int hp = int.Parse(cols[3]);
            var dt = (DamageType)System.Enum.Parse(typeof(DamageType), cols[4].Trim(), true);
            int bd = int.Parse(cols[5]);
            float cc = float.Parse(cols[6]);
            float cb = float.Parse(cols[7]);
            float dc = float.Parse(cols[8]);
            int ar = int.Parse(cols[9]);
            int me = int.Parse(cols[10]);
            float er = float.Parse(cols[11]);
            int ms = int.Parse(cols[12]);
            int hc = int.Parse(cols[13]);
            string pfPath = cols[last].Trim();

            // Load portrait sprite (optional warning if missing)
            var spr = Resources.Load<Sprite>(portrait);
            if (spr == null)
                Debug.LogWarning($"Hero '{id}': portrait '{portrait}' not found in Resources.");

            // create or update the ScriptableObject
            string assetPath = $"{outFolder}/{id}.asset";
            var data = AssetDatabase.LoadAssetAtPath<HeroAttributeData>(assetPath)
                       ?? ScriptableObject.CreateInstance<HeroAttributeData>();

            data.heroId = id;
            data.heroName = name;
            data.portrait = spr;
            data.maxHP = hp;
            data.damageType = dt;
            data.baseDamage = bd;
            data.critChance = cc;
            data.critDamageBonus = cb;
            data.dodgeChance = dc;
            data.armor = ar;
            data.maxEnergy = me;
            data.energyRegenPerTurn = er;
            data.moveSpeed = ms;
            data.heightClimbLimit = hc;
            data.heroPrefabPath = pfPath;

            if (!AssetDatabase.Contains(data))
                AssetDatabase.CreateAsset(data, assetPath);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Imported {lines.Length - 1} heroes from CSV.");
    }
}
