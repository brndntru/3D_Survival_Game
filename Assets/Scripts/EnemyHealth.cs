using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;

    float health;
    bool dead = false;

    public GameObject deathEffect;

    // ui
    public System.Action onHealthChanged;
    public float Health01
    {
        get
        {
            if (maxHealth <= 0f) return 0f;
            return Mathf.Clamp01(health / maxHealth);
        }
    }

    public DamageIndicator damageIndicatorPrefab;
    public Transform indicatorAnchor;  
    public Vector3 indicatorOffset = new Vector3(0.3f, 0.5f, 0f);

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
            Vector3 basePos = indicatorAnchor
                ? indicatorAnchor.position
                : transform.position;

            Vector3 pos = basePos + indicatorOffset;

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
        if (dead) return;
        dead = true;

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        onHealthChanged?.Invoke();

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
            // waits for the death animation to play before removing object
            Destroy(gameObject, 2f); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
