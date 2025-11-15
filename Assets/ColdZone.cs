using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColdZone : MonoBehaviour
{
    [Tooltip("Extra cold per second while inside this zone.")]
    public float coldRate = 8f;

    void Reset()
    {
        var c = GetComponent<Collider>(); c.isTrigger = true;
    }
    void OnTriggerEnter(Collider other)
    {
        var v = other.GetComponentInParent<PlayerVitals>();
        if (v) v.AddColdRate(coldRate);
    }
    void OnTriggerExit(Collider other)
    {
        var v = other.GetComponentInParent<PlayerVitals>();
        if (v) v.AddColdRate(-coldRate);
    }
}
