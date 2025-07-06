// Assets/Scripts/Gameplay/HeroStatsUI.cs
using UnityEngine;
using TMPro;

public class HeroStatsUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assigned by GameplayInitializer at runtime")]
    public HeroController hero;

    [Header("UI Elements")]
    public TMP_Text hpText;
    public TMP_Text armorText;
    public TMP_Text energyText;
    public TMP_Text moveText;

    // No Start() check—just let Update handle the timing

    void Update()
    {
        if (hero == null)
        {
            // still waiting for the hero to be wired in
            return;
        }

        // Once hero is non-null, update every frame
        /*hpText.text = $"HP: {hero.currentHP}/{hero.maxHP}";
        armorText.text = $"Armor: {hero.currentArmor}/{hero.armor}";
        energyText.text = $"Energy: {hero.currentEnergy}/{hero.maxEnergy}";
        moveText.text = $"Speed: {hero.moveDistance}";*/

        hpText.text = $"{hero.currentHP}/{hero.maxHP}";
        armorText.text = $"{hero.currentArmor}";
        moveText.text = $"{hero.moveDistance}"; 


    }
}
