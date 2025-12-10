using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                 // Assign Main Camera
    public Animator animator;          // Optional swing animation

    [Header("Attack Settings")]
    public float damage = 25f;
    public float attackRange = 2.3f;
    public float hitRadius = 0.4f;
    public float attackCooldown = 0.5f;
    public float hitDelay = 0.1f;

    [Header("Input")]
    public int mouseButton = 0;        // Left click
    public bool inputBuffering = true;

    float lastAttackTime = -999f;
    bool isAttacking = false;
    bool buffered = false;

    void Start()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
            else if (inputBuffering)
            {
                buffered = true;
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // trigger animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // wait for animation to reach swing point
        yield return new WaitForSeconds(hitDelay);

        PerformHit();
        lastAttackTime = Time.time;

        // cooldown
        yield return new WaitForSeconds(attackCooldown - hitDelay);

        isAttacking = false;

        if (buffered)
        {
            buffered = false;
            StartCoroutine(AttackRoutine());
        }
    }

    void PerformHit()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.SphereCast(ray, hitRadius, out RaycastHit hit, attackRange))
        {
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
