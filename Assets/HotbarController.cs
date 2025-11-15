using UnityEngine;

public class HotbarController : MonoBehaviour
{
    [Header("Inventories")]
    public Inventory mainInventory;    // Player's main inventory
    public Inventory hotbarInventory;  // The hotbar inventory (size 5–8)

    [Header("Optional hooks")]
    public HotbarSelection selection;  // Drag your HotbarSelection (Player)
    public HandController hand;        // Drag your HandController (for instant unequip)

    [Header("Behavior")]
    public bool autoSelectNewlyFilled = true;     // select/equip when an item lands in hotbar
    public bool autoUnequipOnRemoveSelected = true; // unequip if you remove the selected hotbar slot

    /// <summary>
    /// Click handler: move entire stack from source[index] to the other container.
    /// Wire this from SlotClick.OnPointerClick -> controller.TransferFrom(inventory, index)
    /// </summary>
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

        // Choose the opposite container
        Inventory target = (source == hotbarInventory) ? mainInventory : hotbarInventory;
        if (!target) return;

        int start = slot.count;
        int leftover = target.AddReturnLeftover(slot.item, slot.count);
        int moved = start - leftover;
        if (moved <= 0) return;

        // Update source slot
        if (leftover == 0)
        {
            source.ClearSlot(index);
        }
        else
        {
            slot.count = leftover;
            source.OnInventoryChanged?.Invoke();
        }

        // If we removed from the currently selected hotbar slot, unequip immediately
        if (removingSelected)
        {
            // Only unequip if it actually became empty
            if (index < source.slots.Count && source.slots[index].IsEmpty)
            {
                if (hand) hand.Unequip();
                // Re-fire selection to refresh UI/highlight even if index didn't change
                if (selection) selection.Select(selection.selectedIndex);
            }
        }
    }

    /// <summary>
    /// Use this on pickup: try hotbar first, leftover goes to main.
    /// Optionally selects the slot that received the item.
    /// </summary>
    public void AddPreferHotbar(ItemData item, int amount)
    {
        if (!item || amount <= 0) return;

        int left = hotbarInventory ? hotbarInventory.AddReturnLeftover(item, amount) : amount;
        if (left > 0 && mainInventory) mainInventory.AddReturnLeftover(item, left);

        if (autoSelectNewlyFilled && selection && hotbarInventory)
        {
            // Focus the first hotbar slot that now contains this item
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
