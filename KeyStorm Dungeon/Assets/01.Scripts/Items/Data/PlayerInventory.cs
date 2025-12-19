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
    public int hpPotion;

    [Header("아이템")]
    public List<ItemData> passiveItems = new List<ItemData>();
    public ItemData activeItem;

    // 골드
    public void AddGold(int amount)
    {
        gold += amount;
        if (gold < 0) gold = 0;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;
        gold -= amount;
        return true;
    }

    // 폭탄
    public void AddBomb(int amount)
    {
        bombCount += amount;
        if (bombCount < 0) bombCount = 0;
    }

    public bool TryUseBomb(int amount = 1)
    {
        if (bombCount < amount) return false;
        bombCount -= amount;
        return true;
    }

    //포션
    public void AddPotion(int amount)
    {
        hpPotion += amount;
        if (hpPotion < 0) hpPotion = 0;
    }



    // 포션을 사용해서 회복하는 방식.
    // stats 쪽에 Heal(int) 같은 함수가 있으면 그걸 호출하면 됨.
    public bool TryUsePotion(int healAmount)
    {
        if (hpPotion <= 0) return false;
        if (stats == null) return false;

        // 이미 풀피면 사용 안 하게
        if (stats.hp >= stats.maxHp) return false;

        hpPotion--;
        stats.Heal(healAmount);
        return true;
    }

    //아이템(패시브,액티브)
    public void AddPassiveItem(ItemData data)
    {
        if (data == null) return;

        passiveItems.Add(data);

        if (inventoryModel != null)
            inventoryModel.AddItem(data);

        FindObjectOfType<InventoryUIController>()?.Refresh();
    }

    public void SetActiveItem(ItemData data)
    {
        activeItem = data;
    }
}
