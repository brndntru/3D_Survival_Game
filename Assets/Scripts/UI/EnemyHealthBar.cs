using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth enemy;        
    public Slider slider;             
    public CanvasGroup canvasGroup;   

    [Header("Positioning")]
    public Vector3 worldOffset = new Vector3(0f, 2f, 0f);

    [Header("Visibility")]
    public float showWhenBelow = 0.99f;       
    public float visibleSecondsAfterHit = 2f; 

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

        // face camera 
        if (Camera.main)
        {
            Vector3 camPos = Camera.main.transform.position;
            transform.rotation = Quaternion.LookRotation(transform.position - camPos);
        }

        // handles visibility timer
        if (canvasGroup)
        {
            if (hideTimer > 0f)
            {
                hideTimer -= Time.deltaTime;
                canvasGroup.alpha = 1f;
            }
            else
            {
                canvasGroup.alpha = (enemy.Health01 < showWhenBelow) ? 1f : 0f;
            }
        }
    }

    void OnHealthChanged()
    {
        Refresh();
        hideTimer = visibleSecondsAfterHit;  
    }

    void Refresh()
    {
        if (!enemy || !slider) return;
        slider.value = enemy.Health01;
    }
}
