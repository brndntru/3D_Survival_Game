using UnityEngine;
using System.Collections.Generic;

[System.Serializable] public class InventorySlot { public ItemData item; public int count; public bool IsEmpty => item == null || count <= 0; }

public class Inventory : MonoBehaviour
{
    public int size = 16;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public System.Action OnInventoryChanged;

    void Awake()
    {
        if (slots.Count != size) { slots.Clear(); for (int i = 0; i < size; i++) slots.Add(new InventorySlot()); }
    }

    public bool Add(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // stack first
        if (item.stackable)
        {
            foreach (var s in slots)
            {
                if (s.item == item && s.count < item.maxStack)
                {
                    int add = Mathf.Min(amount, item.maxStack - s.count);
                    s.count += add; amount -= add; if (amount <= 0) { OnInventoryChanged?.Invoke(); return true; }
                }
            }
        }
        // then empty slots
        foreach (var s in slots)
        {
            if (s.IsEmpty)
            {
                int add = item.stackable ? Mathf.Min(amount, item.maxStack) : 1;
                s.item = item; s.count = add; amount -= add; if (amount <= 0) { OnInventoryChanged?.Invoke(); return true; }
            }
        }
        OnInventoryChanged?.Invoke();
        return false; // no space
    }
    public int AddReturnLeftover(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return amount;

        if (item.stackable)
        {
            for (int i = 0; i < slots.Count && amount > 0; i++)
            {
                var s = slots[i];
                if (s.item == item && s.count < item.maxStack)
                {
                    int canAdd = Mathf.Min(amount, item.maxStack - s.count);
                    s.count += canAdd; amount -= canAdd;
                }
            }
        }
        for (int i = 0; i < slots.Count && amount > 0; i++)
        {
            var s = slots[i];
            if (s.IsEmpty)
            {
                int add = item.stackable ? Mathf.Min(amount, item.maxStack) : 1;
                s.item = item; s.count = add; amount -= add;
            }
        }
        OnInventoryChanged?.Invoke();
        return amount; // 0 = all placed
    }

    public void ClearSlot(int index)
    {
        slots[index].item = null;
        slots[index].count = 0;
        OnInventoryChanged?.Invoke();
    }
}
