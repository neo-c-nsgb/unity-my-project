// Assets/Scripts/UIBlock.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBlock : MonoBehaviour
{
    public BlockType Type { get; private set; }

    /// <summary>
    /// Initialize type & visuals. Call immediately after Instantiate().
    /// </summary>
    public void Initialize(BlockType type)
    {
        Type = type;
        var img = GetComponent<Image>();
        switch (type)
        {
            case BlockType.Bomb: img.color = Color.red; break;
            case BlockType.ColorMatch3: img.color = Color.green; break;
            default: img.color = Color.white; break;
        }
    }

    /// <summary>
    /// Hook for special‐block logic (e.g. explosion, match‐3, etc.).
    /// </summary>
    public void OnSpecialTrigger()
    {
        // TODO: implement behavior per BlockType
    }
}