using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitItemUI : MonoBehaviour
{
    public Toggle toggleDone;
    public TMP_Text textName;
    public Button btnDelete;

    private Action<HabitItemUI> onDeleteCb;

    // Configura el ítem con un nombre y callback de borrado
    public void Setup(string habitName, Action<HabitItemUI> onDelete)
    {
        textName.text = string.IsNullOrWhiteSpace(habitName) ? "Nuevo hábito" : habitName.Trim();
        toggleDone.isOn = false;

        onDeleteCb = onDelete;
        btnDelete.onClick.RemoveAllListeners();
        btnDelete.onClick.AddListener(() => onDeleteCb?.Invoke(this));
    }
}