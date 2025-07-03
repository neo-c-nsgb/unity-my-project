// Assets/Scripts/CellButton.cs
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellButton : MonoBehaviour, IPointerClickHandler
{
    public int x, y;
    public BlockType currentType = BlockType.None;

    TMP_Text label;
    static BlockType[] types = (BlockType[])System.Enum.GetValues(typeof(BlockType));

    void Awake()
    {
        label = GetComponentInChildren<TMP_Text>();
        Refresh();
    }

    public void OnPointerClick(PointerEventData e)
    {
        int idx = System.Array.IndexOf(types, currentType);
        if (e.button == PointerEventData.InputButton.Left)
            idx = (idx + 1) % types.Length;
        else if (e.button == PointerEventData.InputButton.Right)
            idx = (idx - 1 + types.Length) % types.Length;
        currentType = types[idx];
        Refresh();
    }

    public void Refresh()
    {
        label.text = currentType == BlockType.None
            ? ""
            : currentType.ToString()[0].ToString();
    }
}
