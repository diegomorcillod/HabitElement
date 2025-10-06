using System;
using UnityEngine;
using UnityEngine.Events;

/// Vigila el cambio de día (local) y emite un evento cuando cambia "yyyy-MM-dd".
public class DayWatcher : MonoBehaviour
{
    [Tooltip("Comprobar cada N segundos (30 recomendado).")]
    public float checkIntervalSeconds = 30f;

    [Serializable] public class DayChangedEvent : UnityEvent { }
    public DayChangedEvent OnDayChanged;

    private string currentDay;
    private float accum;

    private void Awake()
    {
        currentDay = DateTime.Now.ToString("yyyy-MM-dd");
    }

    private void Update()
    {
        accum += Time.unscaledDeltaTime;
        if (accum < checkIntervalSeconds) return;
        accum = 0f;

        var today = DateTime.Now.ToString("yyyy-MM-dd");
        if (today != currentDay)
        {
            currentDay = today;
            OnDayChanged?.Invoke();
        }
    }

    // Por si la app vuelve del background
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) return;
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        if (today != currentDay)
        {
            currentDay = today;
            OnDayChanged?.Invoke();
        }
    }
}
