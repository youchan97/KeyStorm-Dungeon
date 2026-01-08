using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;

    void Awake()
    {
        if (itemIcon == null)
        {
            itemIcon = GetComponent<Image>();
        }
    }

    public void SetItem(AcquiredItemData item)
    {
        if (item != null && item.itemIcon != null && itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
            itemIcon.color = Color.white;
        }
    }
}
