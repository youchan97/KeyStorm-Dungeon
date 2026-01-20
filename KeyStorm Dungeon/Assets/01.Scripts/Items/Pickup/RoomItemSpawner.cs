using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemSpawner : MonoBehaviour
{
    public enum SpawnType { Boss, TreasureOnStart }

    public SpawnType spawnType;

    [Header("생성 위치")]
    public Transform spawnPoint;

    [Header("픽업 프리팹")]
    public GameObject passiveItemPickupPrefab;
    public GameObject activeItemPickupPrefab;

    [Header("중복 방지")]
    public bool spawnOnlyOnce = true;

    private bool spawned;
    private GameObject spawnedObj;

    private void Start()
    {
        if (spawnType == SpawnType.TreasureOnStart)
            SpawnTreasureRoomItem();
    }

    public void SpawnTreasureRoomItem()
    {
        SpawnItem(ItemDropRoom.Treasure);
    }

    public void SpawnBossRoomItem()
    {
        SpawnItem(ItemDropRoom.Boss);
    }

    private void SpawnItem(ItemDropRoom dropRoom)
    {
        if (spawnOnlyOnce && spawned) return;

        if (ItemPoolManager.Instance == null)
        {
            return;
        }

        ItemData data = ItemPoolManager.Instance.GetRandomItem_ExcludeAcquired(dropRoom);
        if (data == null)
        {
            return;
        }

        GameObject prefab = data.isActiveItem ? activeItemPickupPrefab : passiveItemPickupPrefab;
        if (prefab == null)
        {
            return;
        }

        if (spawnedObj != null) Destroy(spawnedObj);

        Transform point = spawnPoint != null ? spawnPoint : transform;

        spawnedObj = Instantiate(prefab, point.position, Quaternion.identity);

        if (spawnedObj.TryGetComponent<PassiveItemPickup>(out var passive))
        {
            passive.itemData = data;
            passive.isShopDisplay = false; 
        }

        if (spawnedObj.TryGetComponent<ActiveItemPickup>(out var active))
        {
            active.itemData = data;
            active.isShopDisplay = false; 
            active.SetData(data); 
        }

        // View 적용
        spawnedObj.GetComponent<ItemPickupView>()?.Apply(data);


        spawned = true;
    }
}
