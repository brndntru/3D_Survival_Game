using UnityEngine;

public class HotbarController : MonoBehaviour
{
    [Header("Inventories")]
    public Inventory mainInventory;    
    public Inventory hotbarInventory;  

    [Header("Optional hooks")]
    public HotbarSelection selection; 
    public HandController hand;       

    [Header("Behavior")]
    public bool autoSelectNewlyFilled = true;    
    public bool autoUnequipOnRemoveSelected = true; 

    public void TransferFrom(Inventory source, int index)
    {
        if (!source) return;
        if (index < 0 || index >= source.slots.Count) return;

        var slot = source.slots[index];
        if (slot == null || slot.IsEmpty) return;

        bool removingSelected =
            autoUnequipOnRemoveSelected &&
            source == hotbarInventory &&
            selection != null &&
            index == selection.selectedIndex;

        Inventory target = (source == hotbarInventory) ? mainInventory : hotbarInventory;
        if (!target) return;

        int start = slot.count;
        int leftover = target.AddReturnLeftover(slot.item, slot.count);
        int moved = start - leftover;
        if (moved <= 0) return;

        if (leftover == 0)
        {
            source.ClearSlot(index);
        }
        else
        {
            slot.count = leftover;
            source.OnInventoryChanged?.Invoke();
        }

        // unequip if removed from currently selected hotbar slot
        if (removingSelected)
        {
            // checks if actually empty
            if (index < source.slots.Count && source.slots[index].IsEmpty)
            {
                if (hand) hand.Unequip();
                if (selection) selection.Select(selection.selectedIndex);
            }
        }
    }

    public void AddPreferHotbar(ItemData item, int amount)
    {
        if (!item || amount <= 0) return;

        int left = hotbarInventory ? hotbarInventory.AddReturnLeftover(item, amount) : amount;
        if (left > 0 && mainInventory) mainInventory.AddReturnLeftover(item, left);

        if (autoSelectNewlyFilled && selection && hotbarInventory)
        {
            for (int i = 0; i < hotbarInventory.slots.Count; i++)
            {
                var s = hotbarInventory.slots[i];
                if (s.item == item)
                {
                    selection.Select(i);
                    break;
                }
            }
        }
    }
}
