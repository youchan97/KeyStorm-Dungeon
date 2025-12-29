using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectedFrame;

    public ItemData Data { get; private set; }

    public void Set(ItemData data)
    {
        Data = data;

        if (Data != null && Data.iconSprite != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = Data.iconSprite;
        }
        else
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }

    public void SetSelected(bool on)
    {
        if (selectedFrame != null)
            selectedFrame.SetActive(on);
    }
}
