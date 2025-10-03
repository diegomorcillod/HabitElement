using UnityEngine;

[ExecuteAlways]
public class SafeArea : MonoBehaviour
{
    public RectTransform target; // normalmente el hijo raíz que quieres encajar (p.ej. Canvas child)
    void OnEnable() { Apply(); }
    void OnRectTransformDimensionsChange() { Apply(); }

    void Apply()
    {
        if (target == null) target = transform as RectTransform;
        var rt = target;
        var sa = Screen.safeArea;

        Vector2 min = sa.position;
        Vector2 max = sa.position + sa.size;

        min.x /= Screen.width;  min.y /= Screen.height;
        max.x /= Screen.width;  max.y /= Screen.height;

        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
