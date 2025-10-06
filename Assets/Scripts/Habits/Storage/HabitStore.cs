using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HabitListWrapper { public List<Habit> items = new(); }

public static class HabitStore
{
    private const string KEY = "habits_json";

    public static List<Habit> Load()
    {
        var json = PlayerPrefs.GetString(KEY, "");
        if (string.IsNullOrEmpty(json)) return new List<Habit>();
        try
        {
            var wrap = JsonUtility.FromJson<HabitListWrapper>(json);
            return wrap?.items ?? new List<Habit>();
        }
        catch { return new List<Habit>(); }
    }

    public static void Save(List<Habit> habits)
    {
        var wrap = new HabitListWrapper { items = habits };
        var json = JsonUtility.ToJson(wrap);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }
}
