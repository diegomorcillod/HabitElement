using UnityEngine;
using TMPro;

public class XpSystem : MonoBehaviour
{
    public TMP_Text xpLabel;
    private const string XP_KEY = "xp_total";

    public int Xp
    {
        get => PlayerPrefs.GetInt(XP_KEY, 0);
        private set { PlayerPrefs.SetInt(XP_KEY, value); PlayerPrefs.Save(); UpdateLabel(); }
    }

    public int Level => Xp / 100; // 100 XP por nivel

    private void Start() => UpdateLabel();

    public void AddXp(int amount)
    {
        Xp = Mathf.Max(0, Xp + amount);
    }

    private void UpdateLabel()
    {
        if (xpLabel) xpLabel.text = $"XP: {Xp}  |  Nivel: {Level}";
    }
}
