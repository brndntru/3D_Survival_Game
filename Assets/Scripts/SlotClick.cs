using UnityEngine;
using UnityEngine.EventSystems;

public class SlotClick : MonoBehaviour, IPointerClickHandler
{
    public Inventory inventory;  
    public int index;
    public HotbarController controller;

    public void Init(Inventory inv, int idx, HotbarController ctl)
    { inventory = inv; index = idx; controller = ctl; }

    public void OnPointerClick(PointerEventData e)
    {
        if (controller != null && inventory != null)
            controller.TransferFrom(inventory, index);
    }
}
