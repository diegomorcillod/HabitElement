using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitUIController : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_InputField inputNewHabit;
    public Transform content;           // ScrollView_Habits/Viewport/Content
    public GameObject habitItemPrefab;  // Prefab HabitItem

    [Header("XP")]
    public XpSystem xpSystem;
    public int xpPerHabit = 10;

    private readonly List<Habit> habits = new();

    private void Start()
    {
        var loaded = HabitStore.Load();
        habits.Clear();
        habits.AddRange(loaded);
        RenderList();
    }

    public void OnAddHabitClicked()
    {
        var name = inputNewHabit?.text?.Trim();
        if (string.IsNullOrEmpty(name) || habitItemPrefab == null || content == null) return;

        habits.Add(new Habit(name));
        HabitStore.Save(habits);
        inputNewHabit.text = "";
        RenderList();
    }

    private void RenderList()
    {
        // Limpia
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        // Crea
        foreach (var h in habits)
        {
            var go = Instantiate(habitItemPrefab, content);
            var ui = go.GetComponent<HabitItemUI>();
            ui.Bind(h, OnToggleChanged, OnDeleteRequested);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }

    private void OnToggleChanged(Habit habit, bool nowOn)
    {
        bool wasDone = habit.IsDoneToday();
        habit.SetDoneToday(nowOn);

        // Da XP sólo cuando pasa de "no hecho" -> "hecho hoy"
        if (!wasDone && nowOn) xpSystem?.AddXp(xpPerHabit);

        HabitStore.Save(habits);
    }

    private void OnDeleteRequested(Habit habit)
    {
        habits.RemoveAll(x => x.id == habit.id);
        HabitStore.Save(habits);
        RenderList();
    }
}
