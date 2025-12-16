using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageManager : MonoBehaviour
{
    [Header("Stage Data")]
    public StageData stageData;

    [Header("Room Prefabs")]
    public List<Room> normalRooms;
    public List<Room> bossRooms;
    public List<Room> treasureRooms;
    public List<Room> shopRooms;

    [Header("Corridor Tilemap")]
    public Tilemap corridorTilemap;
    public TileBase corridorTile;

    [Header("Config")]
    public int roomSpacing = 20;

    // 논리 데이터
    Dictionary<Vector2Int, RoomNode> roomMap = new();
    Dictionary<Vector2Int, Room> spawnedRooms = new();

    readonly Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    void Start()
    {
        GenerateRoomLayout();
        AssignRoomTypes();
        SpawnRooms();
        ConnectRooms();
        //UpdateDoors();
    }

    // =========================================================
    // 1. 방 배치 (퍼져나가는 구조)
    // =========================================================
    void GenerateRoomLayout()
    {
        roomMap.Clear();

        Queue<Vector2Int> queue = new();
        Vector2Int start = Vector2Int.zero;

        roomMap[start] = new RoomNode
        {
            gridPos = start,
            type = RoomType.Normal
        };

        queue.Enqueue(start);

        while (roomMap.Count < stageData.totalRoomCount)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in dirs.OrderBy(_ => Random.value))
            {
                Vector2Int next = current + dir;
                if (roomMap.ContainsKey(next)) continue;

                roomMap[next] = new RoomNode { gridPos = next };
                queue.Enqueue(next);

                if (roomMap.Count >= stageData.totalRoomCount)
                    break;
            }
        }
    }

    // =========================================================
    // 2. 룸 타입 할당
    // =========================================================
    void AssignRoomTypes()
    {
        List<RoomNode> nodes = roomMap.Values.ToList();

        // 시작 방 제외
        nodes.Remove(roomMap[Vector2Int.zero]);

        List<RoomType> typePool = new();

        typePool.AddRange(Enumerable.Repeat(RoomType.Boss, stageData.bossRoomCount));
        typePool.AddRange(Enumerable.Repeat(RoomType.Treasure, stageData.treasureRoomCount));
        typePool.AddRange(Enumerable.Repeat(RoomType.Shop, stageData.shopRoomCount));

        int normalCount = nodes.Count - typePool.Count;
        typePool.AddRange(Enumerable.Repeat(RoomType.Normal, normalCount));

        typePool = typePool.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < nodes.Count; i++)
            nodes[i].type = typePool[i];
    }

    // =========================================================
    // 3. 방 생성
    // =========================================================
    void SpawnRooms()
    {
        foreach (RoomNode node in roomMap.Values)
        {
            Room prefab = GetRandomRoomPrefab(node.type);

            Vector3 worldPos = new Vector3(
                node.gridPos.x * roomSpacing,
                node.gridPos.y * roomSpacing,
                0
            );

            Room room = Instantiate(prefab, worldPos, Quaternion.identity);
            spawnedRooms[node.gridPos] = room;
        }
    }

    Room GetRandomRoomPrefab(RoomType type)
    {
        return type switch
        {
            RoomType.Boss =>
                bossRooms[Random.Range(0, bossRooms.Count)],

            RoomType.Treasure =>
                treasureRooms[Random.Range(0, treasureRooms.Count)],

            RoomType.Shop =>
                shopRooms[Random.Range(0, shopRooms.Count)],

            _ =>
                normalRooms[Random.Range(0, normalRooms.Count)]
        };
    }

    // =========================================================
    // 4. 방 연결 (직선 복도)
    // =========================================================
    void ConnectRooms()
    {
        foreach (var kv in spawnedRooms)
        {
            Vector2Int pos = kv.Key;
            Room room = kv.Value;

            foreach (var dir in dirs)
            {
                Vector2Int next = pos + dir;
                if (!spawnedRooms.ContainsKey(next)) continue;

                // 중복 복도 방지
                if (pos.x > next.x || pos.y > next.y) continue;

                Room other = spawnedRooms[next];

                Transform from = room.GetDoor(dir).transform;
                Transform to = other.GetDoor(-dir).transform;

                DrawCorridor(from.position, to.position);
            }
        }
    }

    // =========================================================
    // 5. 문 자동 차단 / 개방
    // =========================================================
    void UpdateDoors()
    {
        /*foreach (var kv in spawnedRooms)
        {
            Vector2Int pos = kv.Key;
            Room room = kv.Value;

            foreach (var dir in dirs)
            {
                Door door = room.GetDoor(dir);
                if (door == null) continue;

                if (spawnedRooms.ContainsKey(pos + dir))
                    door.Open();
                else
                    door.Close();
            }
        }*/
    }

    // =========================================================
    // 6. 복도 타일 찍기
    // =========================================================
    void DrawCorridor(Vector3 from, Vector3 to)
    {
        Vector3Int start = corridorTilemap.WorldToCell(from);
        Vector3Int end = corridorTilemap.WorldToCell(to);

        if (start.x == end.x)
        {
            int min = Mathf.Min(start.y, end.y);
            int max = Mathf.Max(start.y, end.y);

            for (int y = min; y <= max; y++)
                corridorTilemap.SetTile(new Vector3Int(start.x, y, 0), corridorTile);
        }
        else
        {
            int min = Mathf.Min(start.x, end.x);
            int max = Mathf.Max(start.x, end.x);

            for (int x = min; x <= max; x++)
                corridorTilemap.SetTile(new Vector3Int(x, start.y, 0), corridorTile);
        }
    }
}
