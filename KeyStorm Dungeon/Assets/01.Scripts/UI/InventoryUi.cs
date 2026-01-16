using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    PlayerInventory inventory;


    public void SetInventoryUi(PlayerInventory inventory)
    {
        this.inventory = inventory;
        inventory.OnAddPassiveItem += AddItemImage;

    }

    void AddItemImage(ItemData data)
    {
        
    }


    void SetImage()
    {
        if (inventory == null)
            return;

        
    }
}
