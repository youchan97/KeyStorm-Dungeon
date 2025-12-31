using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemSpawner : MonoBehaviour
{
    public enum AutoSpawnMode { None, TreasureOnStart }

    [Header("자동 스폰")]
    public AutoSpawnMode autoSpawn = AutoSpawnMode.None;

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
        if (autoSpawn == AutoSpawnMode.TreasureOnStart)
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
            Debug.LogError("[RoomItemSpawner] ItemPoolManager.Instance가 NULL");
            return;
        }

        ItemData data = ItemPoolManager.Instance.GetRandomItem_ExcludeAcquired(dropRoom);
        if (data == null)
        {
            Debug.LogWarning($"[RoomItemSpawner] {dropRoom} 풀에 아이템이 없음");
            return;
        }

        GameObject prefab = data.isActiveItem ? activeItemPickupPrefab : passiveItemPickupPrefab;
        if (prefab == null)
        {
            Debug.LogError("[RoomItemSpawner] 픽업 프리팹이 인스펙터에 비어있음");
            return;
        }

        if (spawnedObj != null) Destroy(spawnedObj);

        Transform point = spawnPoint != null ? spawnPoint : transform;

        spawnedObj = Instantiate(prefab, point.position, Quaternion.identity);

        Debug.Log($"════════════════════════════════════════");
        Debug.Log($"[RoomItemSpawner] 아이템 스폰");
        Debug.Log($"  Room: {dropRoom}");
        Debug.Log($"  ItemData: {data.itemName}");
        Debug.Log($"  IsActive: {data.isActiveItem}");
        Debug.Log($"  Position: {point.position}");

        if (spawnedObj.TryGetComponent<PassiveItemPickup>(out var passive))
        {
            passive.itemData = data;
            passive.isShopDisplay = false; 
            Debug.Log($"  → PassiveItemPickup 설정 완료");
            Debug.Log($"     itemData: {passive.itemData.itemName}");
            Debug.Log($"     isShopDisplay: {passive.isShopDisplay}");
        }

        if (spawnedObj.TryGetComponent<ActiveItemPickup>(out var active))
        {
            active.itemData = data;
            active.isShopDisplay = false; 
            active.SetData(data); 
            Debug.Log($"  → ActiveItemPickup 설정 완료");
            Debug.Log($"     itemData: {active.itemData.itemName}");
            Debug.Log($"     isShopDisplay: {active.isShopDisplay}");
        }

        // View 적용
        spawnedObj.GetComponent<ItemPickupView>()?.Apply(data);

        Debug.Log($"════════════════════════════════════════");

        spawned = true;
    }
}
