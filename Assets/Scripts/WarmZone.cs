using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WarmZone : MonoBehaviour
{
    [Tooltip("Warmth per second (use positive value).")]
    public float warmthRate = 12f;

    [Header("Visual (Editor Only)")]
    public Color gizmoColor = new Color(1f, 0.5f, 0f, 0.3f); 
    public bool showGizmo = true;

    private Collider triggerCollider;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning("WarmZone collider should be a trigger!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var v = other.GetComponentInParent<PlayerVitals>();
        if (v)
        {
            v.AddColdRate(-Mathf.Abs(warmthRate));
            Debug.Log($"Player entered warm zone - Warming at {warmthRate}/sec");
        }
    }

    void OnTriggerExit(Collider other)
    {
        var v = other.GetComponentInParent<PlayerVitals>();
        if (v)
        {
            v.AddColdRate(+Mathf.Abs(warmthRate));
            Debug.Log($"Player left warm zone - Stopped warming");
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = gizmoColor;

        if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius * transform.localScale.x);
        }
        else if (col is BoxCollider box)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.matrix = oldMatrix;
        }
        else if (col is CapsuleCollider capsule)
        {
            Gizmos.DrawSphere(transform.position + capsule.center, capsule.radius * transform.localScale.x);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f);

        if (col is SphereCollider sphere)
        {
            Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius * transform.localScale.x);
        }
        else if (col is BoxCollider box)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
            Gizmos.matrix = oldMatrix;
        }
    }
}