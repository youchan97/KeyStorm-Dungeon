using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShopSlot : MonoBehaviour
{
    public enum ShopSlotType
    {
        RandomItem,   // 아이템 풀에서 뽑는 슬롯
        HeartPotion,  // 체력 물약고정
        Bomb          // 폭탄고정
    }

    public ShopSlotType slotType;

    [Header("가격")]
    public int price = 100;

    [Header("생성 위치")]
    public Transform spawnPoint;

    [Header("픽업 프리팹")]
    public GameObject passiveItemPickupPrefab;
    public GameObject activeItemPickupPrefab;

    ItemData itemData;   // RandomItem 슬롯에 걸린 아이템

    private void Start()
    {
        if (slotType == ShopSlotType.RandomItem)
        {
            // 상점용 풀에서 아이템 뽑기
            itemData = ItemPoolManager.Instance.GetRandomItem(ItemDropRoom.Shop);
            SpawnItemPickup();
        }
        else
        {
            // HeartPotion / Bomb 전용 ItemData가 있으면 여기서 GetItemById 해서 itemData 지정해도 됨
        }
    }

    void SpawnItemPickup()
    {
        if (itemData == null) return;

        GameObject prefab = itemData.isActiveItem ? activeItemPickupPrefab : passiveItemPickupPrefab;
        Transform point = spawnPoint != null ? spawnPoint : transform;

        GameObject obj = Instantiate(prefab, point.position, Quaternion.identity);

        if (itemData.isActiveItem)
            obj.GetComponent<ActiveItemPickup>().itemData = itemData;
        else
            obj.GetComponent<PassiveItemPickup>().itemData = itemData;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        // 골드 부족하면 그냥 통과
        if (inv.gold < price)
        {
            // "골드 부족" 출력하고 싶으면 여기에서 하면 됨
            return;
        }

        // 골드 차감
        inv.gold -= price;
        HudUI.Instance.UpdateGold(inv.gold);

        switch (slotType)
        {
            case ShopSlotType.RandomItem:
                // 아이템은 이미 바닥에 있으니까, 여기서는 슬롯만 비활성화
                DisableSlot();
                break;

            case ShopSlotType.HeartPotion:
                // PlayerHealth 회복 로직 여기서
                DisableSlot();
                break;

            case ShopSlotType.Bomb:
                inv.AddBomb(1);
                DisableSlot();
                break;
        }
    }

    void DisableSlot()
    {
        GetComponent<Collider2D>().enabled = false;
        // "SOLD OUT" 스프라이트로 바꾸고 싶으면 여기서 처리하면 됩니당
    }
}
