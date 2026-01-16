using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstValue;

public class InventoryItemSlot : MonoBehaviour
{
    [SerializeField] Image itemIcon;

    [SerializeField] float scaleUpValue;
    [SerializeField] Outline outline;

    Vector3 originScale;

    public ItemData ItemData {  get; private set; }
    public RectTransform RectTransform { get; private set; }

    public void InitData(ItemData data)
    {
        ItemData = data;
        itemIcon.sprite = data.iconSprite;
        RectTransform = GetComponent<RectTransform>();
        originScale = RectTransform.localScale;
        SetSelected(false);
    }

    public void SetSelected(bool isSelect)
    {
        RectTransform.localScale = isSelect ? new Vector3(scaleUpValue, scaleUpValue, originScale.z) : originScale;
        outline.enabled = isSelect;
    }
}
