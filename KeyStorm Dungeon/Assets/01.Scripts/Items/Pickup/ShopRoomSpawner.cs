using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoomSpawner : MonoBehaviour
{
    public enum StoreSlotType
    {
        RandomItem,
        Bomb,
        Potion
    }

    [System.Serializable]
    public class StoreSlotEntry
    {
        public StoreSlot slot;
        public StoreSlotType type;
        public int price;
    }

    [Header("슬롯 엔트리")]
    public List<StoreSlotEntry> entries = new();

    [Header("소모품 설정")]
    public int bombAmount = 1;
    public int potionAmount = 1;
    public int potionHealAmount = 2;

    [Header("프리팹")]
    public GameObject passiveItemPickupPrefab;
    public GameObject activeItemPickupPrefab;
    public GameObject potionDisplayPrefab;
    public GameObject bombDisplayPrefab;

    private void Start()
    {
        SpawnStore(); //이 줄이 없으면 절대 안 뜸
    }

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
                    e.slot.SetConsumableProduct(
                        bombDisplayPrefab,
                        e.price,
                        StoreSlot.ConsumableType.Bomb,
                        bombAmount
                    );
                    break;

                case StoreSlotType.Potion:
                    e.slot.SetConsumableProduct(
                        potionDisplayPrefab,
                        e.price,
                        StoreSlot.ConsumableType.Potion,
                        potionAmount,
                        potionHealAmount
                    );
                    break;
            }
        }
    }

    void SpawnRandomItem(StoreSlot slot, int price)
    {
        var data = ItemPoolManager.Instance
            .GetRandomItem_ExcludeAcquired(ItemDropRoom.Store);

        if (data == null)
        {
            slot.Clear();
            return;
        }

        var prefab = data.isActiveItem
            ? activeItemPickupPrefab
            : passiveItemPickupPrefab;

        slot.SetItemProduct(prefab, data, price);
    }
}
