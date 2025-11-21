// InventorySlotUI.cs
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;

    void Awake()  // start empty & transparent
    {
        if (icon)
        {
            icon.sprite = null;
            icon.enabled = false;
            icon.color = new Color(1f, 1f, 1f, 0f); // transparent
        }
        if (countText) countText.text = "";
    }

    public void Set(Sprite s, int count)
    {
        bool has = s != null;

        if (icon)
        {
            icon.sprite = s;
            icon.enabled = has;
            icon.color = has ? Color.white : new Color(1f, 1f, 1f, 0f); // white when filled, transparent when empty
        }

        if (countText)
            countText.text = (has && count > 1) ? count.ToString() : "";
    }
}
