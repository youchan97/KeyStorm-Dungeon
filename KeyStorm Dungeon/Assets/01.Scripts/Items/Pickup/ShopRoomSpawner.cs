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

    public List<StoreSlotEntry> entries = new();

    public GameObject passiveItemPickupPrefab;
    public GameObject activeItemPickupPrefab;
    public GameObject bombDisplayPrefab;
    public GameObject potionDisplayPrefab;

    private HashSet<string> pickedThisStore = new();

    private void Start()
    {
        SpawnStore();
    }

    public void SpawnStore()
    {
        var pool = ItemPoolManager.Instance;
        if (pool == null) return;

        pickedThisStore.Clear();

        foreach (var e in entries)
        {
            if (e.slot == null) continue;

            if (e.type == StoreSlotType.RandomItem)
            {
                ItemData data = null;

                for (int i = 0; i < 15; i++)
                {
                    var candidate = pool.GetRandomItem_ExcludeAcquired(ItemDropRoom.Store);

                    if (candidate == null) break;
                    if (pickedThisStore.Contains(candidate.itemId)) continue;

                    data = candidate;
                    pickedThisStore.Add(candidate.itemId);
                    break;
                }
                if (data == null) { e.slot.Clear(); continue; }

                var prefab = data.isActiveItem ? activeItemPickupPrefab : passiveItemPickupPrefab;
                e.slot.SetItemProduct(prefab, data, e.price);
            }
            else if (e.type == StoreSlotType.Bomb)
            {
                e.slot.SetConsumableProduct(bombDisplayPrefab, e.price, StoreSlot.ConsumableType.Bomb, 1, 0);
            }
            else if (e.type == StoreSlotType.Potion)
            {
                e.slot.SetConsumableProduct(potionDisplayPrefab, e.price, StoreSlot.ConsumableType.Potion, 1, 0);
            }
        }
    }
}
