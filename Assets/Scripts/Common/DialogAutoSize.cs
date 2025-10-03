using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DialogAutoSize : MonoBehaviour
{
    public RectTransform panelBox;   // asigna Panel_Box
    [Range(0f, 200f)] public float margin = 48f;
    public float minWidth = 560f;
    public float maxWidth = 900f;

    void OnEnable()  { Apply(); }
    void OnRectTransformDimensionsChange() { Apply(); }

    void Apply()
    {
        if (panelBox == null) return;
        var parent = panelBox.parent as RectTransform;
        if (parent == null) return;

        float available = Mathf.Max(0f, parent.rect.width - margin * 2f);
        float target = Mathf.Clamp(available, minWidth, maxWidth);

        panelBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target);
        // alto lo controla el Content Size Fitter según el texto
        panelBox.anchoredPosition = Vector2.zero; // centrado
    }
}
