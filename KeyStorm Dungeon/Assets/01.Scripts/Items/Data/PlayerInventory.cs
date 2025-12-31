using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryRunData runData;
    public PlayerStats stats;

    [Header("자원")]
    public int gold;
    public int bombCount;
    public int hpPotion;

    [Header("아이템")]
    public List<ItemData> passiveItems = new();
    public ItemData activeItem;

    [Header("드롭용 공용 액티브 픽업 프리팹(필수)")]
    public GameObject defaultActivePickupPrefab;

    public void InitInventory(InventoryRunData data)
    {
        gold = data.gold;
        bombCount = data.bombCount;
        passiveItems = data.passiveItems;
        activeItem = data.activeItem;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        if (gold < 0) gold = 0;
        runData.UpdateGold(gold);
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
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;
        gold -= amount;
        return true;
    }
    public void AddPassiveItem(ItemData data)
    {
        if (data == null) return;

        passiveItems.Add(data);
    }

    public void SetActiveItem(ItemData newItem)
    {
        if (newItem == null || !newItem.isActiveItem) return;

        if (activeItem != null)
        {
            DropActiveItem(activeItem);
        }

        activeItem = newItem;
        runData.ApplyInventory(activeItem);
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
            Debug.LogError("[PlayerInventory] defaultActivePickupPrefab가 비어있습니다. PlayerInventory 인스펙터에 ActiveItemPickup 프리팹을 넣어주세요.");
            return;
        }

        Vector2 offset = Random.insideUnitCircle.normalized * 0.6f;
        Vector3 dropPos = transform.position + (Vector3)offset;

        GameObject drop = Instantiate(prefab, dropPos, Quaternion.identity);

        if (drop.TryGetComponent<ActiveItemPickup>(out var pickup))
            pickup.SetData(oldItem);

        drop.GetComponent<ItemPickupView>()?.Apply(oldItem);
    }
}
