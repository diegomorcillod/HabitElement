using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelProgressUI : MonoBehaviour
{
    public Slider slider;
    public TMP_Text label;
    const string XP_KEY = "xp_total"; // misma clave que en tu XpSystem

    void OnEnable() { Refresh(); }

    public void Refresh()
    {
        int xp = PlayerPrefs.GetInt(XP_KEY, 0);
        int level = xp / 100;
        int within = xp % 100;
        if (slider) { slider.minValue = 0; slider.maxValue = 100; slider.value = within; }
        if (label) label.text = $"Nivel {level} · {within}/100 XP";
    }
}
