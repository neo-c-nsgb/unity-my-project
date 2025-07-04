// Assets/Editor/HeroDataImporter.cs
using System.IO;
using UnityEditor;
using UnityEngine;

public static class HeroDataImporter
{
    [MenuItem("Tools/Import Heroes from CSV")]
    public static void ImportFromCSV()
    {
        var csvPath = "Assets/Resources/Data/Heroes.csv";
        if (!File.Exists(csvPath)) { Debug.LogError("CSV not found"); return; }
        var lines = File.ReadAllLines(csvPath);
        if (lines.Length < 2) { Debug.LogError("No data rows"); return; }

        var outFolder = "Assets/Resources/Heroes";
        if (!AssetDatabase.IsValidFolder(outFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "Heroes");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var cols = lines[i].Split(',');

            // parse columns
            var id = cols[0].Trim();
            var name = cols[1].Trim();
            var pPath = cols[2].Trim();
            int hp = int.Parse(cols[3]);
            var dT = (DamageType)System.Enum.Parse(typeof(DamageType), cols[4].Trim(), true);
            int dmg = int.Parse(cols[5]);
            float cc = float.Parse(cols[6]);
            float cb = float.Parse(cols[7]);
            float dc = float.Parse(cols[8]);
            int ar = int.Parse(cols[9]);
            int me = int.Parse(cols[10]);
            float er = float.Parse(cols[11]);
            int ms = int.Parse(cols[12]);
            int hl = int.Parse(cols[13]);
            var pfPath = cols[14].Trim();

            // load assets
            var portrait = Resources.Load<Sprite>(pPath);
            var prefab = Resources.Load<GameObject>(pfPath);

            // create or update SO
            var assetPath = $"{outFolder}/{id}.asset";
            var data = AssetDatabase.LoadAssetAtPath<HeroAttributeData>(assetPath)
                       ?? ScriptableObject.CreateInstance<HeroAttributeData>();

            data.heroId = id;
            data.heroName = name;
            data.portrait = portrait;
            data.maxHP = hp;
            data.damageType = dT;
            data.baseDamage = dmg;
            data.critChance = cc;
            data.critDamageBonus = cb;
            data.dodgeChance = dc;
            data.armor = ar;
            data.maxEnergy = me;
            data.energyRegenPerTurn = er;
            data.moveSpeed = ms;
            data.heightClimbLimit = hl;
            data.heroPrefab = prefab;

            if (!AssetDatabase.Contains(data))
                AssetDatabase.CreateAsset(data, assetPath);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Imported {lines.Length - 1} heroes.");
    }
}
