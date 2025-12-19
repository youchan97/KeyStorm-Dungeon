using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 스폰 정의")]
    // 순서대로 소환 될 몬스터 리스트
    public List<GameObject> monsterPrefabsToSpawn;

    // 순서대로 소환 될 몬스터의 스폰 위치 리스트
    public List<Transform> spawnPoints;

    private bool monstersSpawned = false;

    public void SpawnMonsters()
    {
        if (monstersSpawned)
        {
            return;
        }

        if (monsterPrefabsToSpawn == null || monsterPrefabsToSpawn.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 스폰할 몬스터 프리팹 리스트가 비어 있음");
            return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 스폰 포인트가 할당되지 않음");
            return;
        }

        if (monsterPrefabsToSpawn.Count != spawnPoints.Count)
        {
            Debug.LogError($"스폰 몬스터 갯수와 스폰 포인트가 일치하지 않음");
            return;
        }

        for (int i = 0; i < monsterPrefabsToSpawn.Count; i++)
        {
            GameObject monsterPrefab = monsterPrefabsToSpawn[i];
            Transform spawnPoint = spawnPoints[i];

            if (monsterPrefab == null)
            {
                continue;
            }
            if (spawnPoint == null)
            {
                continue;
            }

            GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);

            spawnedMonster.transform.parent = transform;
        }

        monstersSpawned = true;
    }
}