using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SortMode { PendingFirst = 0, StreakDesc = 1, NameAZ = 2 }

public class HabitUIController : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_InputField inputNewHabit;
    public Transform content;            // ScrollView_Habits/Viewport/Content
    public GameObject habitItemPrefab;   // Prefab HabitItem

    [Header("Sort & Filter")]
    public TMP_Dropdown dropdownSort;    // Dropdown_Sort (Pendientes / Streak / Nombre)
    public Toggle toggleHideDone;        // Toggle_HideDone (“Ocultar hechos hoy”)

    [Header("XP")]
    public XpSystem xpSystem;
    public int xpPerHabit = 10;

    [Header("Dialogs")]
    public ConfirmDialog confirmDialog;  // Instancia en escena

    [Header("System")]
    public DayWatcher dayWatcher;

    // Estado
    private readonly List<Habit> habits = new();

    // Preferencias
    private const string PREF_SORT = "habit.sortMode";
    private const string PREF_HIDE = "habit.hideDone";
    private SortMode sortMode = SortMode.PendingFirst;
    private bool hideDoneToday = false;

    private void Start()
    {
        // Cargar datos
        habits.Clear();
        habits.AddRange(HabitStore.Load());
        Debug.Log($"[HabitUI] Start → cargados {habits.Count} hábitos");

        // Cargar prefs
        sortMode = (SortMode)PlayerPrefs.GetInt(PREF_SORT, (int)SortMode.PendingFirst);
        hideDoneToday = PlayerPrefs.GetInt(PREF_HIDE, 0) == 1;

        // Wiring UI (sort + filtro)
        if (dropdownSort)
        {
            dropdownSort.onValueChanged.RemoveListener(OnSortChanged);
            dropdownSort.value = (int)sortMode;
            dropdownSort.onValueChanged.AddListener(OnSortChanged);
        }
        if (toggleHideDone)
        {
            toggleHideDone.onValueChanged.RemoveListener(OnHideDoneChanged);
            toggleHideDone.isOn = hideDoneToday;
            toggleHideDone.onValueChanged.AddListener(OnHideDoneChanged);
        }

        // DayWatcher
        if (dayWatcher != null)
            dayWatcher.OnDayChanged.AddListener(OnDayChanged);

        RenderList();
    }

    private void OnDestroy()
    {
        if (dayWatcher != null)
            dayWatcher.OnDayChanged.RemoveListener(OnDayChanged);
        if (dropdownSort)   dropdownSort.onValueChanged.RemoveListener(OnSortChanged);
        if (toggleHideDone) toggleHideDone.onValueChanged.RemoveListener(OnHideDoneChanged);
    }

    // Añadir hábito
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

    // Render listado (aplica orden y filtro)
    private void RenderList()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        foreach (var h in GetSorted())
        {
            var go = Instantiate(habitItemPrefab, content);
            var ui = go.GetComponent<HabitItemUI>();
            ui.Bind(h, OnToggleChanged, OnDeleteRequested, OnRenameRequested);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
        Debug.Log($"[HabitUI] RenderList → pintados {content.childCount} elementos (de {habits.Count})");
    }

    // Orden + filtro
    private IEnumerable<Habit> GetSorted()
    {
        IEnumerable<Habit> q = habits;

        if (hideDoneToday)
            q = q.Where(h => !h.IsDoneToday());

        switch (sortMode)
        {
            case SortMode.PendingFirst:
                // Pendientes arriba (false < true), luego por nombre
                q = q.OrderBy(h => h.IsDoneToday())
                     .ThenBy(h => h.name, System.StringComparer.OrdinalIgnoreCase);
                break;

            case SortMode.StreakDesc:
                q = q.OrderByDescending(h => h.streak)
                     .ThenBy(h => h.name, System.StringComparer.OrdinalIgnoreCase);
                break;

            case SortMode.NameAZ:
                q = q.OrderBy(h => h.name, System.StringComparer.OrdinalIgnoreCase);
                break;
        }
        return q;
    }

    // Callbacks UI sort/filtro
    private void OnSortChanged(int v)
    {
        sortMode = (SortMode)v;
        PlayerPrefs.SetInt(PREF_SORT, (int)sortMode);
        PlayerPrefs.Save();
        Debug.Log($"[HabitUI] Sort → {sortMode}");
        RenderList();
    }

    private void OnHideDoneChanged(bool on)
    {
        hideDoneToday = on;
        PlayerPrefs.SetInt(PREF_HIDE, on ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"[HabitUI] HideDone → {hideDoneToday}");
        RenderList();
    }

    // Toggle de un hábito
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
        RenderList(); // reordena si el modo lo requiere
    }

    // Borrado (con confirmación)
    private void OnDeleteRequested(Habit habit)
    {
        Debug.Log($"[HabitUI] Delete requested → «{habit.name}» (id={habit.id})");

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

    // Renombrado
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
        // El propio item refresca su texto; no re-render global
    }

    // Cambio de día / volver del background
    private void OnDayChanged() => RenderList();

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) RenderList();
    }
}
