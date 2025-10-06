using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class LevelRowUI : MonoBehaviour
{
    public enum State { Completed, Current, Locked }

    [Header("Refs")]
    public TMP_Text title;
    public TMP_Text tag;
    public Image stripe;

    [Header("Layout")]
    public float preferredHeight = 100f;
    public float stripeWidth = 14f;

    void Awake()
    {
        // Altura fija de la fila y participar en layout si hubiera uno
        var le = GetComponent<LayoutElement>();
        if (!le) le = gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = false;
        le.preferredHeight = preferredHeight;

        if (stripe)
        {
            var sLE = stripe.GetComponent<LayoutElement>();
            if (!sLE) sLE = stripe.gameObject.AddComponent<LayoutElement>();
            sLE.preferredWidth = stripeWidth;
            stripe.rectTransform.localScale = Vector3.one;
        }

        // Tamaños de texto sensatos
        if (title)
        {
            title.enableAutoSizing = true;
            title.fontSizeMin = 20;
            title.fontSizeMax = 48;
            title.alignment = TextAlignmentOptions.MidlineLeft;

            var tLE = title.GetComponent<LayoutElement>();
            if (!tLE) tLE = title.gameObject.AddComponent<LayoutElement>();
            tLE.flexibleWidth = 1f; // ocupar espacio restante
        }
        if (tag)
        {
            tag.enableAutoSizing = true;
            tag.fontSizeMin = 16;
            tag.fontSizeMax = 36;
            tag.alignment = TextAlignmentOptions.MidlineRight;
        }
    }

    public void Setup(int level, State state)
    {
        if (title) title.text = $"Nivel {level}";
        if (tag)
        {
            tag.text = state switch
            {
                State.Completed => "Completado",
                State.Current   => "Actual",
                _               => "Bloqueado"
            };
        }
        if (stripe)
        {
            var c = state switch
            {
                State.Completed => new Color(0.25f, 0.8f, 0.3f, 1f), // verde
                State.Current   => new Color(1f, 0.75f, 0.2f, 1f),   // ámbar
                _               => new Color(0.6f, 0.6f, 0.6f, 1f),  // gris
            };
            stripe.color = c;
        }
    }
}
