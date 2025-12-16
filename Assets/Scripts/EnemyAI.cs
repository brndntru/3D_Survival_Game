using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseRange = 15f;
    public float stopDistance = 1.5f;

    [Header("Attack")]
    public float damage = 10f;
    public float attackCooldown = 1.2f;

    Animator anim;
    Transform player;
    float lastAttack;

    void Awake()
    {
        // grab references once
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool isMoving = false;

        // chase
        if (dist <= chaseRange && dist > stopDistance)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;

            transform.position += dir.normalized * moveSpeed * Time.deltaTime;

            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            isMoving = true;
        }

        // attack
        if (dist <= stopDistance)
        {
            TryAttack();
        }

        // speed
        if (anim != null)
        {
            anim.SetFloat("Speed", isMoving ? 1f : 0f); 
        }
    }

    void TryAttack()
    {
        if (Time.time < lastAttack + attackCooldown) return;
        lastAttack = Time.time;

        if (anim != null)
            anim.SetTrigger("Attack");

        var vitals = player.GetComponent<PlayerVitals>();
        if (vitals != null)
            vitals.TakeDamage(damage);
    }
}
