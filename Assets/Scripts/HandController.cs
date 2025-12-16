using UnityEngine;

public class HandController : MonoBehaviour
{
    public HotbarSelection selection;  
    public Transform handSocket;    

    GameObject current;
    ItemData currentItem;

    void Start()
    {
        if (!selection) selection = FindObjectOfType<HotbarSelection>();
        if (!handSocket)
        {
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

        // keeps current item if same item(not changed)
        if (nextItem == currentItem) return;

        // clears current
        if (current) Destroy(current);
        currentItem = nextItem;

        // spawns new
        if (currentItem && currentItem.heldPrefab && handSocket)
        {
            current = Instantiate(currentItem.heldPrefab, handSocket);
            current.transform.localPosition = (currentItem.heldLocalPosition == Vector3.zero)
                ? new Vector3(0.3f, -0.35f, 0.6f)
                : currentItem.heldLocalPosition;
            current.transform.localEulerAngles = currentItem.heldLocalRotation;
            if (currentItem.heldLocalScale == Vector3.zero) current.transform.localScale = Vector3.one;
            else current.transform.localScale = currentItem.heldLocalScale;

            //ensures held models don't collide with the player
            foreach (var c in current.GetComponentsInChildren<Collider>()) c.enabled = false;
            foreach (var rb in current.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = true;
            current.layer = LayerMask.NameToLayer("Ignore Raycast"); 
        }
    }
    public void Unequip()
    {
        if (current) Destroy(current);
        currentItem = null;
    }

}
