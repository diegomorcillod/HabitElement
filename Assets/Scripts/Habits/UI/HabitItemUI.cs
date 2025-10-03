using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitItemUI : MonoBehaviour
{
    [Header("UI")]
    public Toggle toggleDone;
    public TMP_Text textName;
    public TMP_InputField inputEditName;   // NUEVO
    public Button btnEdit;                 // NUEVO
    public Button btnDelete;

    private Habit habit;
    private Action<Habit, bool> onToggleCb;
    private Action<Habit> onDeleteCb;
    private Action<Habit, string> onRenameCb;  // NUEVO

    public void Bind(
        Habit habit,
        Action<Habit, bool> onToggle,
        Action<Habit> onDelete,
        Action<Habit, string> onRename      // NUEVO
    )
    {
        this.habit = habit;
        onToggleCb  = onToggle;
        onDeleteCb  = onDelete;
        onRenameCb  = onRename;

        // Visual base
        textName.enableWordWrapping = false;
        textName.overflowMode = TextOverflowModes.Overflow;
        textName.enableAutoSizing = true;
        textName.fontSizeMin = 20;
        textName.fontSizeMax = 60;

        textName.text = habit.name;
        toggleDone.isOn = habit.IsDoneToday();

        // Listeners
        toggleDone.onValueChanged.RemoveAllListeners();
        toggleDone.onValueChanged.AddListener(isOn => onToggleCb?.Invoke(habit, isOn));

        btnDelete.onClick.RemoveAllListeners();
        btnDelete.onClick.AddListener(() => onDeleteCb?.Invoke(habit));

        if (btnEdit != null)
        {
            btnEdit.onClick.RemoveAllListeners();
            btnEdit.onClick.AddListener(EnterEditMode);
        }

        if (inputEditName != null)
        {
            inputEditName.onEndEdit.RemoveAllListeners();
            inputEditName.onEndEdit.AddListener((val) => ConfirmEdit(val));

            // Evita que el Enter agregue salto de línea
            inputEditName.lineType = TMP_InputField.LineType.SingleLine;
        }

        // Asegura estado inicial (modo lectura)
        SetEditing(false);
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
            textName.text = newName;
        }
        SetEditing(false);
    }

    public void CancelEdit() => SetEditing(false);

    private void SetEditing(bool editing)
    {
        if (textName)       textName.gameObject.SetActive(!editing);
        if (inputEditName)  inputEditName.gameObject.SetActive(editing);
    }
}
