using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;         // which container this UI shows
    public GameObject panelRoot;        // panel to show/hide (optional)
    public Transform gridParent;        // Grid holder
    public GameObject slotPrefab;       // SlotUI prefab
    public HotbarController controller; // to move on click
    public bool startHidden = false;
    public KeyCode toggleKey = KeyCode.None; // set Tab for main inv, None for hotbar
    public InventoryPop pop;

    readonly List<InventorySlotUI> slotUIs = new();
    bool built;

    void Start()
    {
        if (panelRoot)
        {               // keep object active; animator handles visibility
            if (!pop) pop = panelRoot.GetComponent<InventoryPop>();
            if (pop) pop.SetClosedImmediate();   // start CLOSED
        }
        Build();
        if (inventory) inventory.OnInventoryChanged += Refresh;
    }

    void OnDestroy() { if (inventory) inventory.OnInventoryChanged -= Refresh; }

    void Update()
    {
        if (panelRoot && toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
        {
            bool willOpen;
            if (pop)
            {
                willOpen = !pop.IsOpen;
                pop.Toggle();
            }
            else
            {
                // fallback if no pop component
                willOpen = !panelRoot.activeSelf;
                panelRoot.SetActive(willOpen);
            }

            // cursor + input lock
            Cursor.visible = willOpen;
            Cursor.lockState = willOpen ? CursorLockMode.None : CursorLockMode.Locked;
            var look = FindObjectOfType<FirstPersonLook>(); if (look) look.enabled = !willOpen;
            var pickup = FindObjectOfType<PlayerPickup>(); if (pickup) pickup.enabled = !willOpen;

            if (willOpen) Refresh();
        }
    }

    void Build()
    {
        if (built || !inventory || !gridParent || !slotPrefab) return;

        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        slotUIs.Clear();
        for (int i = 0; i < inventory.size; i++)
        {
            var go = Instantiate(slotPrefab, gridParent);
            slotUIs.Add(go.GetComponent<InventorySlotUI>());
            var click = go.GetComponent<SlotClick>();
            if (click != null) click.Init(inventory, i, controller);  // <-- binds click
        }
        built = true;
    }

    public void Refresh()
    {
        if (!built || inventory == null) return;
        for (int i = 0; i < slotUIs.Count; i++)
        {
            var s = inventory.slots[i];
            slotUIs[i].Set(s.item ? s.item.icon : null, s.item ? s.count : 0);
        }
    }
}
