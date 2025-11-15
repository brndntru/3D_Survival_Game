using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;                   // drag Main Camera
    public Inventory inventory;          // Player's main Inventory
    public HotbarController controller;  // drag your HotbarControllerGO (or leave null to auto-find)
    public Text promptText;              // optional UI prompt

    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.F;
    public float range = 3f;
    public float aimRadius = 0.4f;       // 0.3–0.6 is forgiving
    public LayerMask pickupMask;         // optional: set to "Pickup" layer

    RaycastHit[] hits = new RaycastHit[8];

    void Start()
    {
        if (!cam) cam = Camera.main;
        if (!inventory) inventory = GetComponent<Inventory>();
        if (!controller) controller = FindObjectOfType<HotbarController>(); // auto-find if not wired
        if (promptText) promptText.text = "";
    }

    void Update()
    {
        var target = FindTarget();

        if (promptText)
            promptText.text = target ? $"Press {pickupKey} to pick up {target.item.displayName}" : "";

        if (target && Input.GetKeyDown(pickupKey))
        {
            // STEP 5: Try hotbar first, leftover to main inventory
            if (controller)
            {
                controller.AddPreferHotbar(target.item, target.amount);
                Destroy(target.gameObject);
            }
            else
            {
                // Fallback if controller missing
                if (inventory && inventory.Add(target.item, target.amount))
                    Destroy(target.gameObject);
            }

            if (promptText) promptText.text = "";
        }
    }

    WorldItem FindTarget()
    {
        int mask = (pickupMask.value == 0) ? Physics.DefaultRaycastLayers : pickupMask.value;

        var ray = new Ray(cam.transform.position, cam.transform.forward);
        int n = Physics.SphereCastNonAlloc(ray, aimRadius, hits, range, mask, QueryTriggerInteraction.Collide);

        WorldItem best = null; float bestDist = Mathf.Infinity;
        for (int i = 0; i < n; i++)
        {
            var wi = hits[i].collider.GetComponentInParent<WorldItem>();
            if (wi && hits[i].distance < bestDist) { bestDist = hits[i].distance; best = wi; }
        }
        if (best) return best;

        // small proximity fallback near crosshair
        var center = cam.transform.position + cam.transform.forward * 1.2f;
        var cols = Physics.OverlapSphere(center, 1.5f, mask, QueryTriggerInteraction.Collide);
        float bestScore = 999f;
        foreach (var c in cols)
        {
            var wi = c.GetComponentInParent<WorldItem>();
            if (!wi) continue;
            Vector3 to = wi.transform.position - cam.transform.position;
            float dist = to.magnitude; if (dist > range) continue;
            float angle = Vector3.Angle(cam.transform.forward, to);
            float score = angle * 0.6f + dist;
            if (score < bestScore) { bestScore = score; best = wi; }
        }
        return best;
    }
}
