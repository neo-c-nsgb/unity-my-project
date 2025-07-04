using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HeroSizeMatcher : MonoBehaviour
{
    public TetrisController tetrisController;

    void Start()
    {
        if (tetrisController == null)
            tetrisController = UnityEngine.Object.FindFirstObjectByType<TetrisController>();
        if (tetrisController == null) return;

        float s = tetrisController.CellSize;
        var rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(s, s);
    }
}
