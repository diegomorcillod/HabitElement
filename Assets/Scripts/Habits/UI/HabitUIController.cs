using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // LayoutRebuilder
using TMPro;

public class HabitUIController : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_InputField inputNewHabit;
    public Transform content;           // ScrollView_Habits/Viewport/Content
    public GameObject habitItemPrefab;  // Prefab HabitItem

    [Header("XP")]
    public XpSystem xpSystem;

    private readonly List<HabitItemUI> items = new();

    public void OnAddHabitClicked()
    {
        if (inputNewHabit == null || content == null || habitItemPrefab == null) return;

        var name = inputNewHabit.text?.Trim();
        if (string.IsNullOrEmpty(name)) return;

        var go = Instantiate(habitItemPrefab, content);
        var ui = go.GetComponent<HabitItemUI>();
        if (ui != null)
        {
            ui.Setup(name, OnItemDeleteRequested);
            items.Add(ui);
        }

        inputNewHabit.text = string.Empty;

        // Refresca layout por si el item no aparece al instante
        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }

    private void OnItemDeleteRequested(HabitItemUI item)
    {
        if (item == null) return;
        items.Remove(item);
        Destroy(item.gameObject);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }
}
