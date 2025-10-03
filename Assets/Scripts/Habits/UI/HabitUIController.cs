using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitUIController : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_InputField inputNewHabit;
    public Transform content;
    public GameObject habitItemPrefab;

    [Header("XP")]
    public XpSystem xpSystem;
    public int xpPerHabit = 10;

    private readonly List<Habit> habits = new();

    private void Start()
    {
        habits.Clear();
        habits.AddRange(HabitStore.Load());
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
        for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);

        foreach (var h in habits)
        {
            var go = Instantiate(habitItemPrefab, content);
            var ui = go.GetComponent<HabitItemUI>();
            ui.Bind(h, OnToggleChanged, OnDeleteRequested, OnRenameRequested);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }

    private void OnToggleChanged(Habit habit, bool nowOn)
    {
        bool wasDone = habit.IsDoneToday();
        habit.SetDoneToday(nowOn);
        if (!wasDone && nowOn) xpSystem?.AddXp(xpPerHabit);
        HabitStore.Save(habits);
    }

    private void OnDeleteRequested(Habit habit)
    {
        habits.RemoveAll(x => x.id == habit.id);
        HabitStore.Save(habits);
        RenderList();
    }

    private void OnRenameRequested(Habit habit, string newName)
    {
        // Actualiza el modelo y persiste
        var idx = habits.FindIndex(x => x.id == habit.id);
        if (idx >= 0) habits[idx].name = newName;
        HabitStore.Save(habits);
        // No hace falta re-render completo; el item ya actualizó su propio texto
    }
}
