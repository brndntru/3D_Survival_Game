using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Text text;
    public float lifetime = 0.7f;
    public float moveUpSpeed = 1f;

    float timer;
    CanvasGroup canvasGroup;

    public void Init(float amount)
    {
        if (!text) text = GetComponentInChildren<Text>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        if (text)
            text.text = Mathf.RoundToInt(amount).ToString();

        timer = lifetime;
        if (canvasGroup) canvasGroup.alpha = 1f;
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        text = GetComponentInChildren<Text>();
    }

    void Update()
    {
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        timer -= Time.deltaTime;

        // floats upwards in world space
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // fades out
        if (canvasGroup)
        {
            float t = Mathf.Clamp01(timer / lifetime);
            canvasGroup.alpha = t;
        }
    }
}
