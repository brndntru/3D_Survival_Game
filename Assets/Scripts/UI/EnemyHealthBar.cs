using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth enemy;         // auto-filled from parent if left empty
    public Slider slider;             // slider on this canvas
    public CanvasGroup canvasGroup;   // for show / hide

    [Header("Positioning")]
    public Vector3 worldOffset = new Vector3(0f, 2f, 0f);

    [Header("Visibility")]
    public float showWhenBelow = 0.99f;       // only show if HP < 99%
    public float visibleSecondsAfterHit = 2f; // how long to stay visible after damage

    float hideTimer;

    void Awake()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!slider) slider = GetComponentInChildren<Slider>();
    }

    void Start()
    {
        if (!enemy) enemy = GetComponentInParent<EnemyHealth>();

        if (slider)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }

        Refresh();

        if (enemy != null)
        {
            enemy.onHealthChanged += OnHealthChanged;
        }
    }

    void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.onHealthChanged -= OnHealthChanged;
        }
    }

    void LateUpdate()
    {
        if (!enemy) return;

        // follow enemy + offset
        transform.position = enemy.transform.position + worldOffset;

        // face camera (billboard)
        if (Camera.main)
        {
            Vector3 camPos = Camera.main.transform.position;
            transform.rotation = Quaternion.LookRotation(transform.position - camPos);
        }

        // handle visibility timer
        if (canvasGroup)
        {
            if (hideTimer > 0f)
            {
                hideTimer -= Time.deltaTime;
                canvasGroup.alpha = 1f;
            }
            else
            {
                // hide if near full HP
                canvasGroup.alpha = (enemy.Health01 < showWhenBelow) ? 1f : 0f;
            }
        }
    }

    void OnHealthChanged()
    {
        Refresh();
        hideTimer = visibleSecondsAfterHit;   // stay visible after getting hit
    }

    void Refresh()
    {
        if (!enemy || !slider) return;
        slider.value = enemy.Health01;
    }
}
