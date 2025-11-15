using UnityEngine;

public class HotbarSelection : MonoBehaviour
{
    public HotbarController controller;     // drag your HotbarControllerGO
    public Inventory hotbar;                // auto-filled from controller if left empty
    [Range(0, 7)] public int selectedIndex = 0;
    public bool wrapScroll = true;

    public System.Action onSelectionChanged;

    void Start()
    {
        if (!hotbar && controller) hotbar = controller.hotbarInventory;
        Select(selectedIndex, invoke: false);
    }

    void Update()
    {
        // 1..8 number keys
        if (Input.GetKeyDown(KeyCode.Alpha1)) Select(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Select(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Select(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Select(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) Select(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) Select(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) Select(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) Select(7);

        // mouse wheel cycle (optional)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) Step(-1);
        if (scroll < 0f) Step(+1);
    }

    void Step(int delta)
    {
        if (!hotbar) return;
        int max = Mathf.Min(8, hotbar.size);
        int next = selectedIndex + delta;
        if (wrapScroll)
        {
            if (next < 0) next = max - 1;
            if (next >= max) next = 0;
        }
        else
        {
            next = Mathf.Clamp(next, 0, max - 1);
        }
        Select(next);
    }

    public void Select(int index, bool invoke = true)
    {
        if (!hotbar) return;
        int max = Mathf.Min(8, hotbar.size);
        selectedIndex = Mathf.Clamp(index, 0, max - 1);
        if (invoke) onSelectionChanged?.Invoke();
    }
}
