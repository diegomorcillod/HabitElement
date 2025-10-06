using UnityEngine;

public class LevelListUI : MonoBehaviour
{
    [Header("UI")]
    public RectTransform content;        // ScrollView_Levels/Viewport/Content
    public GameObject levelRowPrefab;    // Prefabs/UI/LevelRow.prefab

    [Header("Config")]
    public int maxLevels = 10;
    public float rowHeight = 100f;
    public float spacing = 12f;
    public float paddingTop = 16f;
    public float paddingBottom = 24f;

    const string XP_KEY = "xp_total";

    void OnEnable() => Render();

    public void Render()
    {
        if (!content || !levelRowPrefab) return;

        // Limpia
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        int xp = PlayerPrefs.GetInt(XP_KEY, 0);
        int currentLevel = xp / 100;

        float y = -paddingTop; // empezamos bajo el borde superior

        for (int lvl = 1; lvl <= maxLevels; lvl++)
        {
            var go = Instantiate(levelRowPrefab, content);
            var rt = go.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot     = new Vector2(0.5f, 1f);
                rt.localScale = Vector3.one;

                // tamaño y posición de la fila
                rt.sizeDelta = new Vector2(0f, rowHeight);
                rt.anchoredPosition = new Vector2(0f, y);

                y -= (rowHeight + spacing);
            }

            var row = go.GetComponent<LevelRowUI>();
            var state = lvl < currentLevel ? LevelRowUI.State.Completed
                      : lvl == currentLevel ? LevelRowUI.State.Current
                                            : LevelRowUI.State.Locked;
            if (row) row.Setup(lvl, state);
        }

        // Altura total del content para que el scroll funcione
        float total = paddingTop + paddingBottom + maxLevels * rowHeight + (maxLevels - 1) * spacing;
        var size = content.sizeDelta;
        content.sizeDelta = new Vector2(size.x, total);
    }
}
