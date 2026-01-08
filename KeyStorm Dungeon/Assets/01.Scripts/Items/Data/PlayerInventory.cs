using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryRunData runData;
    [SerializeField] Player player;

    [Header("자원")]
    public int gold;
    public int bombCount;
    public int hpPotion;

    [Header("아이템")]
    public List<ItemData> passiveItems = new();
    public ItemData activeItem;

    [Header("드롭용 공용 액티브 픽업 프리팹(필수)")]
    public GameObject defaultActivePickupPrefab;

    public event Action<ItemData> OnAddActiveItem;

    public void InitInventory(InventoryRunData data)
    {
        if (GameManager.Instance.IsCheatMode)
        {
            gold = 999999;
            bombCount = 100;
            passiveItems = data.passiveItems;
            activeItem = data.activeItem;
        }
        else
        {
            gold = data.gold;
            bombCount = data.bombCount;
            passiveItems = data.passiveItems;
            activeItem = data.activeItem;
        }

        RegisterInitialData();
    }

    private void RegisterInitialData()
    {
        if (GameDataManager.Instance == null) return;

        GameDataManager.Instance.SetTotalGold(gold);

        if (activeItem != null && activeItem.iconSprite != null)
        {
            AcquiredItemData itemData = new AcquiredItemData(activeItem.itemName, activeItem.iconSprite);
            GameDataManager.Instance.AddItem(itemData);
        }

        foreach (var item in passiveItems)
        {
            if (item != null && item.iconSprite != null)
            {
                AcquiredItemData itemData = new AcquiredItemData(item.itemName, item.iconSprite);
                GameDataManager.Instance.AddItem(itemData);
            }
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        if (gold < 0) gold = 0;
        runData.UpdateGold(gold);
        player.GameSceneUI.UpdateGold();

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetTotalGold(gold);
        }
    }

    public void AddPotion(int amount)
    {
        hpPotion += amount;
        if (hpPotion < 0) hpPotion = 0;
    }

    public void AddBomb(int amount)
    {
        bombCount += amount;
        if (bombCount < 0) bombCount = 0;
        runData.UpdateBomb(bombCount);
        player.GameSceneUI.UpdateBomb();
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;
        gold -= amount;
        player.GameSceneUI.UpdateGold();

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetTotalGold(gold);
        }

        return true;
    }

    public void AddPassiveItem(ItemData data)
    {
        if (data == null) return;
        passiveItems.Add(data);

        if (GameDataManager.Instance != null && data.iconSprite != null)
        {
            AcquiredItemData itemData = new AcquiredItemData(data.itemName, data.iconSprite);
            GameDataManager.Instance.AddItem(itemData);
        }
    }

    public void SetActiveItem(ItemData newItem)
    {
        if (newItem == null || !newItem.isActiveItem) return;

        if (activeItem != null)
        {
            DropActiveItem(activeItem);
        }

        activeItem = newItem;
        OnAddActiveItem?.Invoke(activeItem);
        runData.ApplyInventory(activeItem);

        if (GameDataManager.Instance != null && newItem.iconSprite != null)
        {
            AcquiredItemData itemData = new AcquiredItemData(newItem.itemName, newItem.iconSprite);
            GameDataManager.Instance.AddItem(itemData);
        }
    }

    private void DropActiveItem(ItemData oldItem)
    {
        if (oldItem == null) return;

        GameObject prefab = ItemDatabase.Instance != null
            ? ItemDatabase.Instance.GetActivePickupPrefab(oldItem.itemId)
            : null;

        if (prefab == null)
            prefab = defaultActivePickupPrefab;

        if (prefab == null)
        {
            Debug.LogError("[PlayerInventory] defaultActivePickupPrefab가 비어있습니다.");
            return;
        }

        Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * 0.6f;
        Vector3 dropPos = transform.position + (Vector3)offset;
        GameObject drop = Instantiate(prefab, dropPos, Quaternion.identity);

        if (drop.TryGetComponent<ActiveItemPickup>(out var pickup))
            pickup.SetData(oldItem);

        drop.GetComponent<ItemPickupView>()?.Apply(oldItem);
    }
}
