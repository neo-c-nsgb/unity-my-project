// Assets/Scripts/HeroView.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(HeroController))]
public class HeroView : MonoBehaviour
{
    [Tooltip("Assign a UI Image prefab (the hero sprite)")]
    public Image spritePrefab;

    [Header("Layout Settings")]
    [Tooltip("Offset of sprite relative to hero center (in cell units)")]
    public Vector2 offset = Vector2.zero;
    [Tooltip("Size multiplier relative to cell size (1 = full cell)")]
    public Vector2 sizeMultiplier = Vector2.one;

    private Image spriteInstance;
    private HeroController heroCtrl;
    private RectTransform spriteRT;

    void Awake()
    {
        heroCtrl = GetComponent<HeroController>();

        if (spritePrefab != null)
        {
            // Instantiate under this hero
            spriteInstance = Instantiate(spritePrefab, transform, false);
            spriteRT = spriteInstance.rectTransform;
            // Center the prefab
            spriteRT.anchorMin = spriteRT.anchorMax = new Vector2(0.5f, 0.5f);
            spriteRT.pivot = new Vector2(0.5f, 0.5f);
            spriteRT.anchoredPosition = Vector2.zero;
        }
    }

    void LateUpdate()
    {
        if (spriteInstance == null) return;

        // Resize according to cell size and multiplier
        float cs = heroCtrl.tetrisController.CellSize;
        spriteRT.sizeDelta = new Vector2(cs * sizeMultiplier.x, cs * sizeMultiplier.y);

        // Apply offset in cell units
        spriteRT.anchoredPosition = new Vector2(offset.x * cs, offset.y * cs);

        // Flip based on facing direction
        spriteRT.localScale = new Vector3(
            heroCtrl.FacingRight ? 1f : -1f,
            1f,
            1f
        );
    }
}
