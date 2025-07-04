// Assets/Scripts/PlayerDataManager.cs
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    public HeroAttributeData[] allHeroes;
    public string selectedHeroId;  // set this before gameplay

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);

        allHeroes = Resources.LoadAll<HeroAttributeData>("Heroes");
        if (string.IsNullOrEmpty(selectedHeroId) && allHeroes.Length > 0)
            selectedHeroId = allHeroes[0].heroId;
    }

    public HeroAttributeData GetSelectedHeroConfig() =>
        System.Array.Find(allHeroes, h => h.heroId == selectedHeroId);
}
