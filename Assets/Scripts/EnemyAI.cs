using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chaseRange = 15f;
    public float stopDistance = 1.5f;
    public float damage = 10f;
    public float attackCooldown = 1.2f;

    Transform player;
    float lastAttack;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= chaseRange && dist > stopDistance)
        {
            Vector3 dir = (player.position - transform.position);
            dir.y = 0; // stay upright
            transform.position += dir.normalized * moveSpeed * Time.deltaTime;

            // face player
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        if (dist <= stopDistance)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttack + attackCooldown)
        {
            lastAttack = Time.time;

            PlayerVitals vitals = player.GetComponent<PlayerVitals>();
            if (vitals != null)
            {
                vitals.TakeDamage(damage);
            }
        }
    }
}
