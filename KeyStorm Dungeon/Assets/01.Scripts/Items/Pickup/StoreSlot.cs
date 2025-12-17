using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StoreSlot : MonoBehaviour
{
    [Header("진열 위치")]
    public Transform spawnPoint;

    [Header("가격")]
    public int price;

    // 현재 진열된 물건
    private GameObject spawnedObj;
    private ItemData itemData;     // 아이템이면 여기 들어옴
    private bool isItemProduct;    // 아이템/소모품 구분

    // 슬롯에 상품을 세팅
    public void SetItemProduct(GameObject pickupPrefab, ItemData data, int _price)
    {
        Clear();

        isItemProduct = true;
        itemData = data;
        price = _price;

        var point = spawnPoint != null ? spawnPoint : transform;
        spawnedObj = Instantiate(pickupPrefab, point.position, Quaternion.identity);

        // pickupPrefab이 Active/Passive 공용일 수도 있으니까 둘 다 시도
        if (spawnedObj.TryGetComponent<PassiveItemPickup>(out var p))
            p.itemData = data;
        if (spawnedObj.TryGetComponent<ActiveItemPickup>(out var a))
            a.itemData = data;
    }

    // 소모품(체력/폭탄 등) 프리팹만 진열하고, 구매 시 별도 지급 처리하면 됨
    public void SetConsumableProduct(GameObject prefab, int _price)
    {
        Clear();

        isItemProduct = false;
        itemData = null;
        price = _price;

        var point = spawnPoint != null ? spawnPoint : transform;
        spawnedObj = Instantiate(prefab, point.position, Quaternion.identity);
    }

    public void Clear()
    {
        if (spawnedObj != null) Destroy(spawnedObj);
        spawnedObj = null;
        itemData = null;
        price = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        // 물건 없으면 무시
        if (spawnedObj == null) return;

        // 돈 없으면 통과(아무것도 안 함)
        if (inv.gold < price) return;

        // 결제
        inv.AddGold(-price);

        if (isItemProduct)
        {
            // 아이템 구매 = "줍는 것"과 동일하게 처리되므로
            // 실제 지급은 pickup이 처리해도 되고, 여기서 직접 처리해도 됨.
            // 여기서는 “pickup 강제 발동” 대신 직접 지급 방식 추천:

            if (itemData != null)
            {
                if (itemData.isActiveItem)
                {
                    // 기존 액티브 드랍은 inv.SetActiveItem 내부/픽업 로직과 맞춰서 택1
                    inv.SetActiveItem(itemData);
                }
                else
                {
                    inv.AddPassiveItem(itemData);
                }

                // 먹었을 때만 다시 안 뜸
                ItemPoolManager.Instance?.MarkAcquired(itemData);
            }

            Clear(); // 산 뒤 슬롯 비우기
        }
        else
        {
            // 소모품 구매 처리(예: 폭탄, 체력물약)
            // 폭탄 예시:
            inv.AddBomb(1);

            Clear();
        }
    }
}
