// InventorySlotUI.cs  (TMP version shown)
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;     // or Text for Legacy

    public void Set(Sprite s, int count)
    {
        if (icon) { icon.sprite = s; icon.enabled = s != null; }
        if (countText) countText.text = (count > 1) ? count.ToString() : "";
    }
}