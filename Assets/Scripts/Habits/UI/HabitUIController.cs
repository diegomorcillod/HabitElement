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

    [Header("Dialogs")]
    public ConfirmDialog confirmDialog; // Asigna la INSTANCIA de escena

    private readonly List<Habit> habits = new();

    private void Start()
    {
        habits.Clear();
        habits.AddRange(HabitStore.Load());
        Debug.Log($"[HabitUI] Start → cargados {habits.Count} hábitos");
        RenderList();
    }

    public void OnAddHabitClicked()
    {
        var name = inputNewHabit?.text?.Trim();
        if (string.IsNullOrEmpty(name) || habitItemPrefab == null || content == null)
        {
            Debug.LogWarning("[HabitUI] OnAddHabitClicked → nombre vacío o refs nulas");
            return;
        }

        habits.Add(new Habit(name));
        HabitStore.Save(habits);
        Debug.Log($"[HabitUI] Añadido hábito → {name}. Total: {habits.Count}");
        inputNewHabit.text = "";
        RenderList();
    }

    private void RenderList()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        foreach (var h in habits)
        {
            var go = Instantiate(habitItemPrefab, content);
            var ui = go.GetComponent<HabitItemUI>();
            ui.Bind(h, OnToggleChanged, OnDeleteRequested, OnRenameRequested);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
        Debug.Log($"[HabitUI] RenderList → pintados {habits.Count} elementos");
    }

    private void OnToggleChanged(Habit habit, bool nowOn)
    {
        bool wasDone = habit.IsDoneToday();
        habit.SetDoneToday(nowOn);

        if (!wasDone && nowOn)
        {
            xpSystem?.AddXp(xpPerHabit);
            Debug.Log($"[HabitUI] Toggle ON → +{xpPerHabit} XP para «{habit.name}»");
        }
        else
        {
            Debug.Log($"[HabitUI] Toggle {(nowOn ? "ON" : "OFF")} → «{habit.name}»");
        }

        HabitStore.Save(habits);
        RenderList();
    }

    private void OnDeleteRequested(Habit habit)
    {
        Debug.Log($"[HabitUI] Delete requested → «{habit.name}» (id={habit.id})");

        // Fallback: si no hay diálogo, borra directo
        if (confirmDialog == null)
        {
            Debug.Log("[HabitUI] No confirmDialog asignado → borrado directo");
            habits.RemoveAll(x => x.id == habit.id);
            HabitStore.Save(habits);
            RenderList();
            return;
        }

        confirmDialog.Show(
            "Eliminar hábito",
            $"¿Eliminar «{habit.name}»? Esta acción no se puede deshacer.",
            onConfirm: () =>
            {
                Debug.Log($"[HabitUI] Confirmed delete → «{habit.name}» (id={habit.id})");
                int removed = habits.RemoveAll(x => x.id == habit.id);
                Debug.Log($"[HabitUI] Eliminados: {removed}. Total ahora: {habits.Count}");
                HabitStore.Save(habits);
                RenderList();
            }
        );
    }

    private void OnRenameRequested(Habit habit, string newName)
    {
        var idx = habits.FindIndex(x => x.id == habit.id);
        if (idx >= 0)
        {
            Debug.Log($"[HabitUI] Rename → «{habits[idx].name}» → «{newName}»");
            habits[idx].name = newName;
            HabitStore.Save(habits);
        }
        else
        {
            Debug.LogWarning($"[HabitUI] Rename → habit id no encontrado: {habit.id}");
        }
        // no hace falta re-render aquí
    }
}

