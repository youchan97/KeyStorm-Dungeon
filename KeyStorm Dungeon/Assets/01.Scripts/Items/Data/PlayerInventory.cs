using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
    public List<ItemData> passiveItems = new();
    public ItemData activeItem;

    [Header("드롭용 공용 액티브 픽업 프리팹(필수)")]
    public GameObject defaultActivePickupPrefab;

    // =====================
    // 골드
    // =====================
    public void AddGold(int amount)
    {
        gold += amount;
        if (gold < 0) gold = 0;
    }

    // =====================
    // 폭탄
    // =====================
    public void AddBomb(int amount)
    {
        bombCount += amount;
        if (bombCount < 0)
            bombCount = 0;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;
        gold -= amount;
        return true;
    }

    // =====================
    // 패시브
    // =====================
    public void AddPassiveItem(ItemData data)
    {
        if (data == null) return;

        passiveItems.Add(data);
        inventoryModel?.AddItem(data);
        FindObjectOfType<InventoryUIController>()?.Refresh();
    }

    // =====================
    // 액티브
    // =====================
    public void SetActiveItem(ItemData newItem)
    {
        if (newItem == null || !newItem.isActiveItem) return;

        // 기존 액티브가 있으면 바닥에 드롭
        if (activeItem != null)
        {
            DropActiveItem(activeItem);
        }

        // 새 액티브 장착
        activeItem = newItem;
    }

    private void DropActiveItem(ItemData oldItem)
    {
        if (oldItem == null) return;

        // itemId 전용 프리팹 먼저 시도(있으면 사용)
        GameObject prefab = ItemDatabase.Instance != null
            ? ItemDatabase.Instance.GetActivePickupPrefab(oldItem.itemId)
            : null;

        //없으면 공용 프리팹 사용(반드시 연결)
        if (prefab == null)
            prefab = defaultActivePickupPrefab;

        if (prefab == null)
        {
            Debug.LogError("[PlayerInventory] defaultActivePickupPrefab가 비어있습니다. PlayerInventory 인스펙터에 ActiveItemPickup 프리팹을 넣어주세요.");
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
