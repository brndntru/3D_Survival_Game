using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVitals : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Cold / Freeze")]
    public float maxFreeze = 100f;     // 0 = warm, 100 = frozen
    public float freeze = 0f;

    [Tooltip("Constant cold per second everywhere. Set 0 to rely only on zones/weather.")]
    public float ambientColdPerSec = 1f;

    [Tooltip("Damage per second to health while fully frozen.")]
    public float damagePerSecWhenFrozen = 5f;

    [Tooltip("Max speed you can warm per second when in a warm zone (negative net cold).")]
    public float warmRecoveryLimitPerSec = 10f;

    [Header("Safety (optional)")]
    [Tooltip("Seconds after spawn where freeze damage is ignored.")]
    public float spawnInvulnSeconds = 0f;
    [Tooltip("Must remain fully frozen this long before health starts ticking.")]
    public float freezeDamageDelay = 0f;

    // game over settings
    [Header("Game Over")]
    [Tooltip("Scene to load when player dies.")]
    public string gameOverSceneName = "GameOver";
    [Tooltip("Delay in seconds before loading game over scene.")]
    public float deathDelay = 1f;

    float coldRateBonus = 0f; 
    public System.Action onVitalsChanged;

    public float Health01 => maxHealth <= 0 ? 0 : Mathf.Clamp01(health / maxHealth);
    public float Freeze01 => maxFreeze <= 0 ? 0 : Mathf.Clamp01(freeze / maxFreeze);

    float spawnTimer;
    float fullFreezeTimer;
    bool isDead = false; 

    void Start()
    {
        spawnTimer = 0f;
        fullFreezeTimer = 0f;
        isDead = false;
        health = Mathf.Clamp(health, 0f, maxHealth);
        freeze = Mathf.Clamp(freeze, 0f, maxFreeze);
        onVitalsChanged?.Invoke();
    }

    void Update()
    {
        if (isDead) return; 

        float dt = Time.deltaTime;
        spawnTimer += dt;

        // cold accumulation
        float netCold = ambientColdPerSec + coldRateBonus;
        if (netCold >= 0f)
            freeze += netCold * dt;
        else
            freeze += Mathf.Max(netCold, -warmRecoveryLimitPerSec) * dt;

        freeze = Mathf.Clamp(freeze, 0f, maxFreeze);

        if (freeze >= maxFreeze) fullFreezeTimer += dt;
        else fullFreezeTimer = 0f;

        // health damage
        if (freeze >= maxFreeze &&
            spawnTimer >= spawnInvulnSeconds &&
            fullFreezeTimer >= freezeDamageDelay &&
            damagePerSecWhenFrozen > 0f)
        {
            health = Mathf.Clamp(health - damagePerSecWhenFrozen * dt, 0f, maxHealth);
        }

        if (health <= 0 && !isDead)
        {
            Die();
        }

        onVitalsChanged?.Invoke();
    }

    void Die()
    {
        isDead = true;

        // disables player controls
        var movementController = GetComponent<MovementController>();
        if (movementController) movementController.enabled = false;

        var firstPersonLook = FindObjectOfType<FirstPersonLook>();
        if (firstPersonLook) firstPersonLook.enabled = false;

        var playerPickup = GetComponent<PlayerPickup>();
        if (playerPickup) playerPickup.enabled = false;

        // loads game over scene
        Invoke(nameof(LoadGameOverScene), deathDelay);
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; 
        amount = Mathf.Abs(amount);
        health = Mathf.Clamp(health - amount, 0f, maxHealth);
        onVitalsChanged?.Invoke();
    }

    public void Heal(float amount)
    {
        health = Mathf.Clamp(health + Mathf.Abs(amount), 0f, maxHealth);
        onVitalsChanged?.Invoke();
    }

    public void AddColdRate(float deltaPerSec)
    {
        coldRateBonus += deltaPerSec;
    }

    public void AddFreeze(float amount)
    {
        freeze = Mathf.Clamp(freeze + amount, 0f, maxFreeze);
        onVitalsChanged?.Invoke();
    }
}