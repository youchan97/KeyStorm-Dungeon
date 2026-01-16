using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour
{
    PlayerInventory inventory;
    [SerializeField] Transform itemSlots;
    [SerializeField] GameObject itemSlot;
    List<ItemData> passiveItems = new List<ItemData>();
    ItemData activeItem;

    private void OnEnable()
    {
        SelectItem();
    }

    private void OnDisable()
    {
        RemoveEvent();
    }

    public void SetInventoryUi(PlayerInventory inventory)
    {
        this.inventory = inventory;
        InitEvent();
    }

    void InitEvent()
    {
        inventory.OnAddPassiveItem += AddPassiveItem;
        inventory.OnAddActiveItem += AddActiveItem;
    }

    void RemoveEvent()
    {
        inventory.OnAddPassiveItem -= AddPassiveItem;
        inventory.OnAddActiveItem -= AddActiveItem;
    }

    void AddPassiveItem(ItemData data)
    {
        if (passiveItems.Contains(data))
            return;

        passiveItems.Add(data);
        Image itemImage = Instantiate(itemSlot, itemSlots).GetComponent<Image>();
        itemImage.sprite = data.iconSprite;
    }

    void AddActiveItem(ItemData data)
    {
        if (activeItem == data)
            return;

        activeItem = data;
    }

    void SelectItem(int index = 0)
    {
        if (passiveItems.Count == 0 && activeItem == null)
            return;
        else if (passiveItems.Count == 0)
        {
            ViewItemInfo(activeItem);
            return;
        }

        ViewItemInfo(passiveItems[index]);

    }

    void ViewItemInfo(ItemData data)
    {
        if (data == null)
            return;


        
    }
}
