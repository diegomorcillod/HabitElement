using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressController : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text totalText;
    public TMP_Text doneTodayText;
    public Slider   todayBar;
    public TMP_Text activeText;
    public TMP_Text bestStreakText;
    public TMP_Text top3Text;
    public TMP_Text xpText;

    // Mantén estos nombres si usas XpSystem tal cual te lo pasé
    const string KEY_XP = "xp.current";
    const string KEY_LV = "xp.level";

    void Start()
    {
        var habits = LoadHabits();

        int total = habits.Count;
        int done  = habits.Count(h => h.IsDoneToday());
        int active = habits.Count(h => h.streak > 0);
        int bestStreak = habits.Count > 0 ? habits.Max(h => h.streak) : 0;

        // Top 3 por racha
        var top3 = habits
            .OrderByDescending(h => h.streak)
            .ThenBy(h => h.name, System.StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList();

        // XP/Nivel desde PlayerPrefs (mismo formato que XpSystem)
        int currentXp = PlayerPrefs.GetInt(XpSystem.KEY_XP, 0);
        int level     = Mathf.Max(1, PlayerPrefs.GetInt(XpSystem.KEY_LV, 1));
        int needed    = 100 + 25 * (level - 1); // misma fórmula

        // UI
        if (totalText)       totalText.text       = total.ToString();
        if (doneTodayText)   doneTodayText.text   = $"{done} / {total}";
        if (todayBar)
        {
            todayBar.interactable = false;
            todayBar.minValue = 0f; todayBar.maxValue = 1f;
            todayBar.value = (total > 0) ? (float)done / total : 0f;
        }
        if (activeText)      activeText.text      = active.ToString();
        if (bestStreakText)  bestStreakText.text  = bestStreak.ToString();
        if (top3Text)        top3Text.text        = BuildTop3Text(top3);
        if (xpText)          xpText.text          = $"XP: {currentXp} / {needed}  |  Nivel: {level}";
    }

    List<Habit> LoadHabits()
    {
        var list = HabitStore.Load();
        return list ?? new List<Habit>();
    }

    int XpNeededForLevel(int lv) => 100 + 25 * (lv - 1);

    string BuildTop3Text(List<Habit> top3)
    {
        if (top3.Count == 0) return "—";
        var sb = new StringBuilder();
        for (int i = 0; i < top3.Count; i++)
        {
            var h = top3[i];
            sb.AppendLine($"{i+1}. {h.name} — {h.streak}");
        }
        return sb.ToString().TrimEnd();
    }
}
