using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Cold / Freeze")]
    public float maxFreeze = 100f;   // 0 = fine, 100 = fully frozen
    public float freeze = 0f;

    [Tooltip("Constant cold per second everywhere. Set 0 to rely only on zones/weather.")]
    public float ambientColdPerSec = 1f;

    [Tooltip("Damage per second to health while fully frozen.")]
    public float damagePerSecWhenFrozen = 5f;

    [Tooltip("Max speed you can warm per second when in a warm zone (negative net cold).")]
    public float warmRecoveryLimitPerSec = 10f;

    float coldRateBonus = 0f; // added by zones (blizzard, water, wind) or campfire (negative)
    public System.Action onVitalsChanged;

    public float Health01 => maxHealth <= 0 ? 0 : Mathf.Clamp01(health / maxHealth);
    public float Freeze01 => maxFreeze <= 0 ? 0 : Mathf.Clamp01(freeze / maxFreeze);

    // ---------------- Camera Shake ----------------
    [Header("Shake Tuning")]
    [Tooltip("How much shake to add per 10 HP lost.")]
    public float shakePer10HP = 0.28f;
    public float minShake = 0.12f;
    public float maxShake = 0.55f;

    [Header("Freeze Tick Gating (smooth the micro shakes)")]
    [Tooltip("Freeze damage accumulates until this HP loss, then we fire one shake.")]
    public float minDamageForShake = 0.5f;
    [Tooltip("Minimum seconds between shakes caused by freeze tick.")]
    public float shakeCooldown = 0.20f;

    [Header("Cold Gate (require cold before any shake)")]
    [Tooltip("If true, we only shake when Freeze01 >= threshold (affects both direct hits & freeze tick).")]
    public bool shakeRequiresCold = true;
    [Range(0f, 1f)] public float freezeShakeThreshold01 = 0.5f;  // 0.5 = half frozen

    // Runtime trackers
    float nextShakeTime;
    float healthDropAccum;   // accumulate freeze-tick losses
    float lastHealth;

    void Start()
    {
        lastHealth = health;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // ----- Cold accumulation (+ makes you freeze, - warms you) -----
        float netCold = ambientColdPerSec + coldRateBonus;
        if (netCold >= 0f) freeze += netCold * dt;
        else freeze += Mathf.Max(netCold, -warmRecoveryLimitPerSec) * dt;

        freeze = Mathf.Clamp(freeze, 0f, maxFreeze);

        // ----- Health damage while fully frozen -----
        if (freeze >= maxFreeze && damagePerSecWhenFrozen > 0f)
        {
            health = Mathf.Clamp(health - damagePerSecWhenFrozen * dt, 0f, maxHealth);
        }

        // ----- Freeze tick shake (gated & smoothed) -----
        float lost = lastHealth - health;
        if (lost > 0f && freeze >= maxFreeze) // only the passive freeze drain path
        {
            // Require cold gate before we react at all
            if (!shakeRequiresCold || Freeze01 >= freezeShakeThreshold01)
            {
                healthDropAccum += lost;  // collect tiny drips
                if (healthDropAccum >= minDamageForShake && Time.unscaledTime >= nextShakeTime)
                {
                    float trauma = Mathf.Clamp(minShake + (healthDropAccum / 10f) * shakePer10HP, minShake, maxShake);
                    CameraShake.I?.AddTrauma(trauma);
                    nextShakeTime = Time.unscaledTime + shakeCooldown;
                    healthDropAccum = 0f;
                }
            }
        }
        else if (lost > 0f)
        {
            // Not a freeze tick (e.g., someone changed health outside TakeDamage). We ignore here.
            healthDropAccum = 0f;
        }

        lastHealth = health;
        onVitalsChanged?.Invoke();
    }

    // ---------------- Public API ----------------
    public void TakeDamage(float amount)
    {
        amount = Mathf.Abs(amount);
        health = Mathf.Clamp(health - amount, 0f, maxHealth);

        // Direct hits: only shake if we're cold enough (per your request)
        if (!shakeRequiresCold || Freeze01 >= freezeShakeThreshold01)
        {
            float trauma = Mathf.Clamp(minShake + (amount / 10f) * shakePer10HP, minShake, maxShake);
            CameraShake.I?.AddTrauma(trauma);
            // Also apply cooldown for direct hits? Usually no, but you can if needed:
            // nextShakeTime = Time.unscaledTime + shakeCooldown;
        }

        onVitalsChanged?.Invoke();
    }

    public void Heal(float amount)
    {
        health = Mathf.Clamp(health + Mathf.Abs(amount), 0f, maxHealth);
        onVitalsChanged?.Invoke();
    }

    public void AddColdRate(float deltaPerSec) // zones call this (positive = colder, negative = warmer)
    {
        coldRateBonus += deltaPerSec;
    }

    public void AddFreeze(float amount)
    {
        freeze = Mathf.Clamp(freeze + amount, 0f, maxFreeze);
        onVitalsChanged?.Invoke();
    }
}
