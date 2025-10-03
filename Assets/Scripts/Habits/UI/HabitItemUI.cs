using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitItemUI : MonoBehaviour
{
    public Toggle toggleDone;
    public TMP_Text textName;
    public Button btnDelete;

    private Habit boundHabit;
    private Action<Habit, bool> onToggleCb;
    private Action<Habit> onDeleteCb;

    public void Bind(Habit habit, Action<Habit, bool> onToggle, Action<Habit> onDelete)
    {
        boundHabit = habit;
        onToggleCb = onToggle;
        onDeleteCb = onDelete;

        textName.enableWordWrapping = false;
        textName.overflowMode = TextOverflowModes.Overflow;
        textName.enableAutoSizing = true;
        textName.fontSizeMin = 20;
        textName.fontSizeMax = 60;

        textName.text = habit.name;
        toggleDone.isOn = habit.IsDoneToday();

        toggleDone.onValueChanged.RemoveAllListeners();
        toggleDone.onValueChanged.AddListener(isOn => onToggleCb?.Invoke(boundHabit, isOn));

        btnDelete.onClick.RemoveAllListeners();
        btnDelete.onClick.AddListener(() => onDeleteCb?.Invoke(boundHabit));
    }
}
