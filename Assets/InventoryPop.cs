using UnityEngine;
using System.Collections;

public class InventoryPop : MonoBehaviour
{
    [Header("Targets")]
    public RectTransform target;          // usually this RectTransform
    public CanvasGroup canvasGroup;       // UI alpha & input

    [Header("Timing")]
    public float openDuration = 0.18f;
    public float closeDuration = 0.15f;

    [Header("Scale")]
    public Vector3 closedScale = new Vector3(0.85f, 0.85f, 1f);
    public AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Coroutine anim;
    bool isOpen;

    void Awake()
    {
        if (!target) target = GetComponent<RectTransform>();
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        SetClosedImmediate();
    }

    public void SetClosedImmediate()
    {
        isOpen = false;
        target.localScale = closedScale;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetOpenImmediate()
    {
        isOpen = true;
        target.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Open()
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Animate(true));
    }

    public void Close()
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Animate(false));
    }

    public void Toggle() { if (isOpen) Close(); else Open(); }
    public bool IsOpen => isOpen;

    IEnumerator Animate(bool opening)
    {
        isOpen = opening;
        float t = 0f;
        float d = opening ? openDuration : closeDuration;

        // start values
        Vector3 fromS = opening ? closedScale : Vector3.one;
        Vector3 toS = opening ? Vector3.one : closedScale;
        float fromA = opening ? 0f : 1f;
        float toA = opening ? 1f : 0f;

        // allow clicks only when visible
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = opening;

        while (t < d)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / d);
            float e = opening ? openCurve.Evaluate(p) : closeCurve.Evaluate(p);
            target.localScale = Vector3.LerpUnclamped(fromS, toS, e);
            canvasGroup.alpha = Mathf.LerpUnclamped(fromA, toA, e);
            yield return null;
        }

        target.localScale = toS;
        canvasGroup.alpha = toA;
        canvasGroup.interactable = opening;
        canvasGroup.blocksRaycasts = opening;
        anim = null;
    }
}
