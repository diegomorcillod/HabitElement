using UnityEngine;
using UnityEngine.UI;

public class SpriteLoopAnimator : MonoBehaviour
{
    public Image target;
    public Sprite[] frames;
    [Range(1f, 60f)] public float fps = 10f;

    int idx; float t;

    void OnEnable()
    {
        if (!target) target = GetComponent<Image>();
        idx = 0; t = 0f;
        if (target && frames != null && frames.Length > 0)
            target.sprite = frames[0];
    }

    void Update()
    {
        if (!target || frames == null || frames.Length == 0 || fps <= 0f) return;
        t += Time.deltaTime;
        float step = 1f / fps;
        while (t >= step)
        {
            t -= step;
            idx = (idx + 1) % frames.Length;
            target.sprite = frames[idx];
        }
    }
}
