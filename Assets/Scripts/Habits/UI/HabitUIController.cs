using UnityEngine;
using TMPro;

public class HabitUIController : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_InputField inputNewHabit;
    public Transform content;           // ScrollView_Habits/Viewport/Content
    public GameObject habitItemPrefab;  // Prefab HabitItem

    [Header("XP")]
    public XpSystem xpSystem;

    // Botón "+ Añadir" llamará a esto. De momento no hace nada (evita NullRefs).
    public void OnAddHabitClicked() { /* lógica vendrá después */ }
}
