using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;       
    public GameObject panelRoot;       
    public Transform gridParent;        
    public GameObject slotPrefab;      
    public HotbarController controller; 
    public bool startHidden = false;
    public KeyCode toggleKey = KeyCode.None; 
    public InventoryPop pop;

    readonly List<InventorySlotUI> slotUIs = new();
    bool built;

    void Start()
    {
        if (panelRoot)
        {             
            if (!pop) pop = panelRoot.GetComponent<InventoryPop>();
            if (pop) pop.SetClosedImmediate();   
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
            if (click != null) click.Init(inventory, i, controller); 
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
