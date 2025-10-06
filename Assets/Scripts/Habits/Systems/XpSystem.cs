using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable] public class IntEvent : UnityEvent<int> { }
[System.Serializable] public class XpChangedEvent : UnityEvent<int,int,int> { } // current, needed, level

public class XpSystem : MonoBehaviour
{
    [Header("UI opcional")]
    public TMP_Text xpLabel;  // Asigna Text_XP si quieres que lo actualice aquí

    [Header("Estado (persistente)")]
    [SerializeField] private int currentXp = 0;   // XP dentro del nivel actual
    [SerializeField] private int level     = 1;   // Nivel actual (min 1)

    [Header("Eventos")]
    public XpChangedEvent OnXpChanged; // (current, needed, level)
    public IntEvent       OnLevelUp;   // new level

    // Claves nuevas (las usamos en TODAS las escenas)
    public const string KEY_XP = "xp.current";
    public const string KEY_LV = "xp.level";

    // Clave antigua (tu sistema previo)
    private const string LEGACY_TOTAL = "xp_total";

    private void Awake()
    {
        // 1) Si existen las nuevas claves, las leemos
        if (PlayerPrefs.HasKey(KEY_XP) && PlayerPrefs.HasKey(KEY_LV))
        {
            currentXp = PlayerPrefs.GetInt(KEY_XP, 0);
            level     = Mathf.Max(1, PlayerPrefs.GetInt(KEY_LV, 1));
        }
        // 2) Si no, migramos desde xp_total (tu sistema viejo con 100 por nivel)
        else if (PlayerPrefs.HasKey(LEGACY_TOTAL))
        {
            int total = PlayerPrefs.GetInt(LEGACY_TOTAL, 0);
            MigrateFromLegacyTotal(total);
            // (opcional) puedes borrar la clave antigua:
            // PlayerPrefs.DeleteKey(LEGACY_TOTAL);
        }
        else
        {
            currentXp = 0;
            level     = 1;
        }

        Save();
        Raise();
        UpdateLabel();
    }

    // Fórmula del nivel: 100, 125, 150, ...
    public int XpNeededForLevel(int lv) => 100 + 25 * (lv - 1);
    public int XpNeededCurrentLevel()   => XpNeededForLevel(level);
    public int GetLevel()               => level;
    public int GetCurrentXp()           => currentXp;

    public void AddXp(int amount)
    {
        if (amount <= 0) return;

        int xp = currentXp + amount;
        int needed = XpNeededCurrentLevel();
        bool leveled = false;

        while (xp >= needed)
        {
            xp -= needed;
            level++;
            leveled = true;
            OnLevelUp?.Invoke(level);
            needed = XpNeededForLevel(level);
        }

        currentXp = xp;
        Save();
        Raise();
        UpdateLabel();
        if (leveled) Debug.Log($"[XP] Level Up → {level}");
    }

    private void Save()
    {
        PlayerPrefs.SetInt(KEY_XP, currentXp);
        PlayerPrefs.SetInt(KEY_LV, level);
        PlayerPrefs.Save();
    }

    private void Raise()
    {
        OnXpChanged?.Invoke(currentXp, XpNeededForLevel(level), level);
    }

    private void UpdateLabel()
    {
        if (!xpLabel) return;
        xpLabel.text = $"XP: {currentXp} / {XpNeededForLevel(level)}  |  Nivel: {level}";
    }

    /// <summary>
    /// Migración desde tu sistema antiguo: xp_total con 100 por nivel.
    /// Reparte ese total según la fórmula nueva (100,125,150,...).
    /// </summary>
    private void MigrateFromLegacyTotal(int total)
    {
        int lv = 1;
        int remaining = total;

        while (true)
        {
            int need = XpNeededForLevel(lv);
            if (remaining >= need) { remaining -= need; lv++; }
            else break;
        }

        level     = Mathf.Max(1, lv);
        currentXp = Mathf.Max(0, remaining);
        Debug.Log($"[XP] Migrado desde xp_total={total} ⇒ level={level}, currentXp={currentXp}");
    }
}

