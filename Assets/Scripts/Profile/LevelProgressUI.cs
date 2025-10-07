using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelProgressUI : MonoBehaviour
{
    public Slider slider;        // Slider_Level
    public TMP_Text label;       // Text_LevelLabel

    private const string XP_KEY = "xp_total";   // origen de verdad simple
    private const int XP_PER_LEVEL = 100;       // 100 por nivel (modelo sencillo)

    void OnEnable()  { Refresh(); }
    void OnApplicationFocus(bool hasFocus) { if (hasFocus) Refresh(); }

    public void Refresh()
    {
        int xpTotal = PlayerPrefs.GetInt(XP_KEY, 0);
        int level   = xpTotal / XP_PER_LEVEL;
        int within  = xpTotal % XP_PER_LEVEL;

        if (slider)
        {
            slider.minValue = 0;
            slider.maxValue = XP_PER_LEVEL;
            slider.value    = within;
            slider.interactable = false;
        }

        if (label)
            label.text = $"Nivel {level} · {within}/{XP_PER_LEVEL} XP";
    }
}
