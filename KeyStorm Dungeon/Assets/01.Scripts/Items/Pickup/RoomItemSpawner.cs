using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemSpawner : MonoBehaviour
{
    [Header("생성 위치")]
    public Transform spawnPoint; // 아이템이 나타날 위치

    [Header("픽업 프리팹")]
    public GameObject passiveItemPickupPrefab; // PassiveItemPickup 붙은 프리팹
    public GameObject activeItemPickupPrefab;  // ActiveItemPickup 붙은 프리팹

    // 보물방: 패시브/액티브 포함 랜덤 아이템 1개 생성
    public void SpawnTreasureRoomItem()
    {
        SpawnItem(ItemDropRoom.Treasure);
    }

    // 보스방 클리어 후: 보스방 전용 드랍 아이템 생성
    public void SpawnBossRoomItem()
    {
        SpawnItem(ItemDropRoom.Boss);
    }

    // 상점: 상점 아이템 슬롯 생성
    public void SpawnStoreItem()
    {
        SpawnItem(ItemDropRoom.Store);
    }

    private void SpawnItem(ItemDropRoom dropRoom)
    {
        // 풀에서 아이템 하나 뽑기 (먹은 건 제외)
        ItemData data = ItemPoolManager.Instance.GetRandomItem_ExcludeAcquired(dropRoom);
        if (data == null)
        {
            Debug.LogWarning($"[RoomItemSpawner] {dropRoom} 에서 뽑을 아이템이 없음");
            return;
        }

        // 패시브 / 액티브 여부에 따라 프리팹 선택
        GameObject prefabToSpawn = data.isActiveItem ? activeItemPickupPrefab : passiveItemPickupPrefab;
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("[RoomItemSpawner] 프리팹이 설정되지 않음");
            return;
        }

        // 아이템 생성
        Transform point = spawnPoint != null ? spawnPoint : transform;
        GameObject obj = Instantiate(prefabToSpawn, point.position, Quaternion.identity);

        // 생성된 Pickup에 ItemData 연결 + (있으면) 스프라이트 적용
        if (data.isActiveItem)
        {
            var pickup = obj.GetComponent<ActiveItemPickup>();
            if (pickup != null) pickup.itemData = data;
        }
        else
        {
            var pickup = obj.GetComponent<PassiveItemPickup>();
            if (pickup != null) pickup.itemData = data;
        }
        obj.GetComponent<ItemPickupView>()?.Apply(data);
        
    }
}
