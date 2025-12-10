using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;

    float health;
    bool dead = false;

    public GameObject deathEffect;

    // ---- NEW: for UI ----
    public System.Action onHealthChanged;
    public float Health01
    {
        get
        {
            if (maxHealth <= 0f) return 0f;
            return Mathf.Clamp01(health / maxHealth);
        }
    }

    // ---- NEW: damage indicator integration ----
    public DamageIndicator damageIndicatorPrefab;
    public Transform indicatorAnchor;   // optional; can leave empty
    public Vector3 indicatorOffset = new Vector3(0.7f, 1.5f, 0f);

    void Start()
    {
        health = Mathf.Clamp(maxHealth, 0f, maxHealth);
        onHealthChanged?.Invoke();
    }

    public void TakeDamage(float amount)
    {
        if (dead) return;

        amount = Mathf.Abs(amount);
        health -= amount;

        // spawn indicator
        if (damageIndicatorPrefab != null)
        {
            // Base position: anchor if set, otherwise enemy position
            Vector3 basePos = indicatorAnchor
                ? indicatorAnchor.position
                : transform.position;

            // Apply local-space offset (right + up)
            Vector3 worldOffset = transform.TransformVector(indicatorOffset);

            Vector3 pos = basePos + worldOffset;

            var popup = Instantiate(damageIndicatorPrefab, pos, Quaternion.identity);
            popup.Init(amount);
        }


        onHealthChanged?.Invoke();

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        onHealthChanged?.Invoke();

        Destroy(gameObject);
    }
}
