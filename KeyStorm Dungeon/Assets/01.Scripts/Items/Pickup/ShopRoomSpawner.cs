using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoomSpawner : MonoBehaviour
{ 
    public enum StoreSlotType
    {
        RandomItem,   // Store 풀에서 랜덤 아이템
        Bomb,         // 고정 폭탄
        Potion        // 고정 물약
    }

    [System.Serializable]
    public class StoreSlotEntry
    {
        public StoreSlot slot;
        public StoreSlotType type;
        public int price;
    }

    [Header("슬롯 구성(원하는 만큼 추가 가능)")]
    public List<StoreSlotEntry> entries = new List<StoreSlotEntry>();

    [Header("아이템 픽업 프리팹(공용)")]
    public GameObject passiveItemPickupPrefab;
    public GameObject activeItemPickupPrefab;

    [Header("소모품 진열 프리팹")]
    public GameObject potionDisplayPrefab;
    public GameObject bombDisplayPrefab;

    public void SpawnStore()
    {
        foreach (var e in entries)
        {
            if (e.slot == null) continue;

            switch (e.type)
            {
                case StoreSlotType.RandomItem:
                    SpawnRandomItem(e.slot, e.price);
                    break;

                case StoreSlotType.Bomb:
                    e.slot.SetConsumableProduct(bombDisplayPrefab, e.price);
                    break;

                case StoreSlotType.Potion:
                    e.slot.SetConsumableProduct(potionDisplayPrefab, e.price);
                    break;
            }
        }
    }

    void SpawnRandomItem(StoreSlot slot, int price)
    {
        var data = ItemPoolManager.Instance.GetRandomItem_ExcludeAcquired(ItemDropRoom.Store);
        if (data == null)
        {
            slot.Clear();
            return;
        }

        var prefab = data.isActiveItem ? activeItemPickupPrefab : passiveItemPickupPrefab;
        slot.SetItemProduct(prefab, data, price);
    }
}
