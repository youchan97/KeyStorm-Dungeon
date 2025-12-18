using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public PlayerStats stats;
    public InventoryModel inventoryModel;

    [Header("자원")]
    public int gold;
    public int bombCount;

    [Header("아이템")]
    public List<ItemData> passiveItems = new List<ItemData>();
    public ItemData activeItem;

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public void AddBomb(int amount)
    {
        bombCount += amount;
    }

    public void AddPassiveItem(ItemData data)
    {
        passiveItems.Add(data);
        inventoryModel.AddItem(data);
        FindObjectOfType<InventoryUIController>()?.Refresh();
    }

    public void SetActiveItem(ItemData data)
    {
        activeItem = data;
    }
}
