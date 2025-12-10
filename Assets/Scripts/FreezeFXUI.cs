using UnityEngine;
using UnityEngine.UI;

public class FreezeFXUI : MonoBehaviour
{
    public PlayerVitals vitals;     // your PlayerVitals
    public Image overlay;           // the FrostOverlay Image
    [Range(0f, 1f)] public float maxAlpha = 0.75f;
    public AnimationCurve alphaByFreeze = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Color coldTint = new Color(0.62f, 0.83f, 1f, 1f); // icy blue

    void Reset()
    {
        overlay = GetComponent<Image>();
    }

    void Start()
    {
        if (!vitals) vitals = FindObjectOfType<PlayerVitals>();
        Apply();
        if (vitals) vitals.onVitalsChanged += Apply;
    }

    void OnDestroy()
    {
        if (vitals) vitals.onVitalsChanged -= Apply;
    }

    void Apply()
    {
        if (!overlay || !vitals) return;
        float t = Mathf.Clamp01(vitals.Freeze01);
        float a = alphaByFreeze.Evaluate(t) * maxAlpha;

        // keep original RGB from coldTint but drive Alpha
        Color c = coldTint;
        c.a = a;
        overlay.color = c;
    }
}
