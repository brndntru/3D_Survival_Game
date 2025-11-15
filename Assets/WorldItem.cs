using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldItem : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;
    void Reset() { var c = GetComponent<Collider>(); c.isTrigger = true; }
}
