using UnityEngine;
using TMPro;

public class XpSystem : MonoBehaviour
{
    public TMP_Text xpLabel;

    private void Awake()
    {
        // Valor por defecto para evitar ver texto vacío
        if (xpLabel) xpLabel.text = "XP: 0  |  Nivel: 0";
    }
}
