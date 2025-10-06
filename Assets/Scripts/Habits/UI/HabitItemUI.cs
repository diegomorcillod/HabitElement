using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitItemUI : MonoBehaviour
{
    [Header("UI")]
    public Toggle toggleDone;
    public TMP_Text textName;

    // Edición inline (opcional)
    public TMP_InputField inputEditName;
    public Button btnEdit;

    // Racha (icono + número)
    public Image streakIcon;       // ← asigna el Image "Icon_Streak"
    public TMP_Text streakLabel;   // ← asigna el TMP "Text_Streak" (solo número)

    public Button btnDelete;

    private Habit habit;
    private Action<Habit, bool> onToggleCb;
    private Action<Habit> onDeleteCb;
    private Action<Habit, string> onRenameCb;

    public void Bind(
        Habit habit,
        Action<Habit, bool> onToggle,
        Action<Habit> onDelete,
        Action<Habit, string> onRename
    )
    {
        this.habit = habit;
        onToggleCb  = onToggle;
        onDeleteCb  = onDelete;
        onRenameCb  = onRename;

        // Nombre
        if (textName)
        {
            // No envolver; usamos el layout para cortar si no cabe
            textName.textWrappingMode = TextWrappingModes.NoWrap;
            textName.overflowMode     = TextOverflowModes.Overflow;
            textName.enableAutoSizing = true;
            textName.fontSizeMin      = 20;
            textName.fontSizeMax      = 60;
            textName.text             = habit.name;
        }

        // Toggle
        if (toggleDone)
        {
            toggleDone.onValueChanged.RemoveAllListeners();
            toggleDone.isOn = habit.IsDoneToday();
            toggleDone.onValueChanged.AddListener(isOn =>
            {
                onToggleCb?.Invoke(habit, isOn);
                ApplyDoneStyle(); // feedback inmediato sin esperar re-render global
            });
        }

        // Borrar
        if (btnDelete)
        {
            btnDelete.onClick.RemoveAllListeners();
            btnDelete.onClick.AddListener(() => onDeleteCb?.Invoke(habit));
        }

        // Editar (opcional)
        if (btnEdit)
        {
            btnEdit.onClick.RemoveAllListeners();
            btnEdit.onClick.AddListener(EnterEditMode);
        }
        if (inputEditName)
        {
            inputEditName.onEndEdit.RemoveAllListeners();
            inputEditName.onEndEdit.AddListener(val => ConfirmEdit(val));
            inputEditName.lineType = TMP_InputField.LineType.SingleLine;
        }

        UpdateStreakBadge();
        ApplyDoneStyle();
        SetEditing(false);
    }

    private void UpdateStreakBadge()
    {
        bool hasStreak = habit != null && habit.streak > 0;

        if (streakIcon)
            streakIcon.gameObject.SetActive(hasStreak);

        if (streakLabel)
        {
            streakLabel.gameObject.SetActive(hasStreak);
            if (hasStreak) streakLabel.text = habit.streak.ToString();
        }
    }

    // Aplica estilo visual para "hecho hoy": tachado + opacidad
    private void ApplyDoneStyle()
    {
        bool done = habit != null && habit.IsDoneToday();

        if (textName)
        {
            // Tachado on/off (bitwise sobre fontStyle)
            var fs = textName.fontStyle;
            fs = done ? (fs | FontStyles.Strikethrough)
                      : (fs & ~FontStyles.Strikethrough);
            textName.fontStyle = fs;

            // Atenuar solo el nombre (no toco CanvasGroup global)
            var c = textName.color;
            c.a = done ? 0.6f : 1f;
            textName.color = c;
        }
    }

    private void EnterEditMode()
    {
        if (inputEditName == null) return;
        inputEditName.text = habit.name;
        SetEditing(true);
        inputEditName.Select();
        inputEditName.ActivateInputField();
        inputEditName.caretPosition = inputEditName.text.Length;
    }

    private void ConfirmEdit(string newName)
    {
        newName = (newName ?? "").Trim();
        if (!string.IsNullOrEmpty(newName) && newName != habit.name)
        {
            onRenameCb?.Invoke(habit, newName);
            if (textName) textName.text = newName;
        }
        SetEditing(false);
    }

    public void Refresh(Habit h)
    {
        habit = h;
        if (textName)   textName.text   = habit.name;
        if (toggleDone) toggleDone.isOn = habit.IsDoneToday();
        UpdateStreakBadge();
        ApplyDoneStyle();
    }

    private void SetEditing(bool editing)
    {
        if (textName)      textName.gameObject.SetActive(!editing);
        if (inputEditName) inputEditName.gameObject.SetActive(editing);
    }
}
