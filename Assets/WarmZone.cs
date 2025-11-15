using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WarmZone : MonoBehaviour
{
    [Tooltip("Warmth per second (use positive value).")]
    public float warmthRate = 12f;

    void Reset()
    {
        var c = GetComponent<Collider>(); c.isTrigger = true;
    }
    void OnTriggerEnter(Collider other)
    {
        var v = other.GetComponentInParent<PlayerVitals>();
        if (v) v.AddColdRate(-Mathf.Abs(warmthRate));
    }
    void OnTriggerExit(Collider other)
    {
        var v = other.GetComponentInParent<PlayerVitals>();
        if (v) v.AddColdRate(+Mathf.Abs(warmthRate));
    }
}
