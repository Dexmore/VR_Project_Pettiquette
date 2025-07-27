using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;

    public void InitItem(ItemData data)
    {
        if (data == null)
        {
            iconImage.enabled = false;
            return;
        }

        iconImage.sprite = data.itemIcon;
        iconImage.enabled = true;
    }
}
