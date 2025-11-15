using UnityEngine;

public class HandController : MonoBehaviour
{
    public HotbarSelection selection;   // drag from Player
    public Transform handSocket;        // child under Main Camera

    GameObject current;
    ItemData currentItem;

    void Start()
    {
        if (!selection) selection = FindObjectOfType<HotbarSelection>();
        if (!handSocket)
        {
            // Try to auto-find a child named "HandSocket"
            var cam = Camera.main ? Camera.main.transform : null;
            if (cam) handSocket = cam.Find("HandSocket");
            if (!handSocket && cam)
            {
                var go = new GameObject("HandSocket");
                handSocket = go.transform; handSocket.SetParent(cam, false);
                handSocket.localPosition = new Vector3(0.3f, -0.35f, 0.6f);
                handSocket.localEulerAngles = Vector3.zero;
                handSocket.localScale = Vector3.one;
            }
        }

        if (selection)
        {
            selection.onSelectionChanged += Redraw;
            if (selection.hotbar) selection.hotbar.OnInventoryChanged += Redraw;
        }
        Redraw();
    }

    void OnDestroy()
    {
        if (selection)
        {
            selection.onSelectionChanged -= Redraw;
            if (selection.hotbar) selection.hotbar.OnInventoryChanged -= Redraw;
        }
    }

    void Redraw()
    {
        if (selection == null || selection.hotbar == null) return;

        var slot = selection.hotbar.slots[selection.selectedIndex];
        var nextItem = slot.item;

        // If item type didn't change, keep current (you could refresh on count if you want)
        if (nextItem == currentItem) return;

        // clear current
        if (current) Destroy(current);
        currentItem = nextItem;

        // spawn new
        if (currentItem && currentItem.heldPrefab && handSocket)
        {
            current = Instantiate(currentItem.heldPrefab, handSocket);
            current.transform.localPosition = (currentItem.heldLocalPosition == Vector3.zero)
                ? new Vector3(0.3f, -0.35f, 0.6f)
                : currentItem.heldLocalPosition;
            current.transform.localEulerAngles = currentItem.heldLocalRotation;
            if (currentItem.heldLocalScale == Vector3.zero) current.transform.localScale = Vector3.one;
            else current.transform.localScale = currentItem.heldLocalScale;

            // Make sure held models don't collide with the player
            foreach (var c in current.GetComponentsInChildren<Collider>()) c.enabled = false;
            foreach (var rb in current.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = true;
            current.layer = LayerMask.NameToLayer("Ignore Raycast"); // optional
        }
    }
    public void Unequip()
    {
        if (current) Destroy(current);
        currentItem = null;
    }

}
