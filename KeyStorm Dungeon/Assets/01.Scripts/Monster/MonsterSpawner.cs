using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 스폰 정의")]
    // 소환 될 몬스터 리스트
    public List<GameObject> monsterPrefabsToSpawn;
    // 소환 될 몬스터의 소환 위치 리스트
    public List<Transform> spawnPoints;

    // 해당 방에서 몬스터가 소환 된 적 있는지
    private bool monstersSpawned = false;

    // 이 몬스터 스포너가 들어갈 Room
    private Room parentRoom;

    private void Awake()
    {
        parentRoom = GetComponentInParent<Room>();
        if (parentRoom == null) return;
    }

    public void SpawnMonsters()
    {
        // 해당 방에 몬스터가 소환 된 적 있다면 반환
        if (monstersSpawned)
        {
            return;
        }

        // 현재 몬스터 방이 전투중임을 설정
        parentRoom.ChangeIsFighting(true);

        if (monsterPrefabsToSpawn == null || monsterPrefabsToSpawn.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 소환할 몬스터 프리팹 리스트가 비어 있음");
            return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 소환 포인트가 할당되지 않음");
            return;
        }

        if (monsterPrefabsToSpawn.Count != spawnPoints.Count)
        {
            Debug.LogError($"소환 몬스터 갯수와 소환 포인트가 일치하지 않음");
            return;
        }

        // 몬스터 리스트만큼 반복
        for (int i = 0; i < monsterPrefabsToSpawn.Count; i++)
        {
            // 몬스터 리스트 i번째의 몬스터 프리팹을 가져옴
            GameObject monsterPrefab = monsterPrefabsToSpawn[i];
            // 소환 위치 리스트 i번째의 소환 위치를 가져옴
            Transform spawnPoint = spawnPoints[i];

            // 소환 위치에 몬스터 프리팹을 생성
            GameObject spawnedMonsterGO = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
            spawnedMonsterGO.transform.parent = transform;

            // 소환한 몬스터의 Monster 컴포넌트를 가져온 뒤
            Monster spawnedMonster = spawnedMonsterGO.GetComponent<Monster>();

            if(spawnedMonster != null)
            {
                // 소환한 몬스터에게 몬스터 스포너가 속한 방을 전달
                spawnedMonster.SetMyRoom(parentRoom);
                // 몬스터 스포너가 속한 방에 몬스터 리스트 추가(몬스터의 생사여부 확인용 리스트)
                parentRoom.AddMonster(spawnedMonster);
            }
        }

        // 몬스터 소환을 완료함
        monstersSpawned = true;
    }
}