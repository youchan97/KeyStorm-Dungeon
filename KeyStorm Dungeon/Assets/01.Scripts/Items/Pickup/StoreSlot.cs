using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StoreSlot : MonoBehaviour
{
    public enum ConsumableType 
    { 
        None, 
        Bomb, 
        Potion 
    }

    [Header("진열 위치")]
    public Transform spawnPoint;

    [Header("가격")]
    public int price;

    private GameObject spawnedObj;

    private bool isItemProduct;
    private ItemData itemData;

    private ConsumableType consumableType;
    private int consumableAmount;
    private int potionHealAmount;

    public void Clear()
    {
        if (spawnedObj != null) Destroy(spawnedObj);
        spawnedObj = null;

        isItemProduct = false;
        itemData = null;

        consumableType = ConsumableType.None;
        consumableAmount = 0;
        potionHealAmount = 0;

        price = 0;
    }

    // 랜덤 아이템 1칸 진열
    public void SetItemProduct(GameObject pickupPrefab, ItemData data, int _price)
    {
        Clear();

        isItemProduct = true;
        itemData = data;
        price = _price;

        var point = spawnPoint != null ? spawnPoint : transform;
        spawnedObj = Instantiate(pickupPrefab, point.position, Quaternion.identity);

        if (spawnedObj.TryGetComponent<PassiveItemPickup>(out var p)) p.itemData = data;
        if (spawnedObj.TryGetComponent<ActiveItemPickup>(out var a)) a.itemData = data;

        // 상점에서 안 보이던 문제 해결 핵심
        spawnedObj.GetComponent<ItemPickupView>()?.Apply(data);
    }

    // 소모품 1칸 진열 (폭탄/포션)
    public void SetConsumableProduct(GameObject displayPrefab, int _price, ConsumableType type, int amount, int healAmount = 0)
    {
        Clear();

        isItemProduct = false;
        itemData = null;

        price = _price;
        consumableType = type;
        consumableAmount = amount;
        potionHealAmount = healAmount;

        var point = spawnPoint != null ? spawnPoint : transform;
        spawnedObj = Instantiate(displayPrefab, point.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        // 물건 없으면 무시
        if (spawnedObj == null) return;

        // 돈 없으면 "통과"
        if (inv.gold < price) return;

        // 결제
        inv.AddGold(-price);

        if (isItemProduct)
        {
            if (itemData == null) { Clear(); return; }

            if (itemData.isActiveItem) inv.SetActiveItem(itemData);
            else inv.AddPassiveItem(itemData);

            ItemPoolManager.Instance?.MarkAcquired(itemData);
            Clear();
        }
        else
        {
            // 소모품 지급(즉시 적용/증가)
            switch (consumableType)
            {
                case ConsumableType.Bomb:
                    inv.AddBomb(consumableAmount);
                    break;

                case ConsumableType.Potion:
                    if (inv.stats != null)
                        inv.stats.Heal(potionHealAmount); // 즉시 회복
                    break;
            }

            Clear();
        }
    }
}
