using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public GameObject highlight;

    public void Set(ItemData data)
    {
        if (data == null || data.iconSprite == null)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0); // 안 보이게
            return;
        }

        icon.sprite = data.iconSprite;
        icon.color = Color.white;
    }

    public void SetSelected(bool selected)
    {
        if (highlight != null) highlight.SetActive(selected);
    }
}
