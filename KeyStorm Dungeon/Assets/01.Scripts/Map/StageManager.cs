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
    /*public Room startRoom;
    public List<Room> normalRooms;
    public List<Room> bossRooms;
    public List<Room> treasureRooms;
    public List<Room> shopRooms;*/

    [Header("Corridor Tilemap")]
    public Tilemap corridorTilemap;
    public TileBase corridorTile;

    [Header("Config")]
    public int roomSpacing = 20;
    public int corridorWidth;

    Dictionary<Vector2Int, RoomNode> roomMap = new Dictionary<Vector2Int, RoomNode>();
    Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();

    readonly Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    void Start()
    {
        stageData = StageDataManager.Instance.CurrentStageData;

        GenerateRoomLayout();
        SpawnRooms();
        ConnectRooms();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            GameManager.Instance.StageClear();
        }
    }

    void GenerateRoomLayout()
    {
        roomMap.Clear();

        Queue<Vector2Int> queue = new();
        Vector2Int start = Vector2Int.zero;

        roomMap[start] = new RoomNode
        {
            gridPos = start,
            type = RoomType.Start
        };

        queue.Enqueue(start);

        while (roomMap.Count < stageData.totalRoomCount)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in dirs.OrderBy(_ => Random.value))
            {
                Vector2Int next = current + dir;
                if (roomMap.ContainsKey(next)) continue;

                roomMap[next] = new RoomNode { gridPos = next, type = RoomType.Normal };
                queue.Enqueue(next);

                if (roomMap.Count >= stageData.totalRoomCount)
                    break;
            }
        }
    }

    void SpawnRooms()
    {
        spawnedRooms.Clear();

        int specialTotal = stageData.bossRoomCount + stageData.treasureRoomCount + stageData.shopRoomCount;

        if (specialTotal > roomMap.Count -1) return;

        List<Room> spawnPool = new List<Room>();

        spawnPool.AddRange(PickRandom(StageDataManager.Instance.CurrentStageSet.bossRooms, stageData.bossRoomCount));
        spawnPool.AddRange(PickRandom(StageDataManager.Instance.CurrentStageSet.treasureRooms, stageData.treasureRoomCount));
        spawnPool.AddRange(PickRandom(StageDataManager.Instance.CurrentStageSet.shopRooms, stageData.shopRoomCount));

        int normalCount = roomMap.Count - spawnPool.Count -1;
        spawnPool.AddRange(PickRandom(StageDataManager.Instance.CurrentStageSet.normalRooms, normalCount));

        spawnPool = spawnPool.OrderBy(_ => Random.value).ToList();

        int i = 0;
        foreach (RoomNode node in roomMap.Values)
        {
            Vector3 worldPos = new(
                node.gridPos.x * roomSpacing,
                node.gridPos.y * roomSpacing,
                0
            );

            Room curRoom;
            switch (node.type)
            {
                case RoomType.Start:
                    curRoom = StageDataManager.Instance.CurrentStageSet.startRoom;
                    break;

                default:
                    curRoom = spawnPool[i++];
                    break;
            }

            Room room = Instantiate(curRoom, worldPos, Quaternion.identity);
            spawnedRooms[node.gridPos] = room;
        }
    }

    List<Room> PickRandom(List<Room> source, int count)
    {
        return source.OrderBy(_ => Random.value).Take(count).ToList();
    }

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

                if (pos.x > next.x || pos.y > next.y) continue;

                Room other = spawnedRooms[next];

                Transform from = room.GetDoor(dir).transform;
                Transform to = other.GetDoor(-dir).transform;

                DrawCorridor(from.position, to.position);
            }
        }
    }

    void DrawCorridor(Vector3 from, Vector3 to)
    {
        Vector3Int start = corridorTilemap.WorldToCell(from);
        Vector3Int end = corridorTilemap.WorldToCell(to);

        int offsetMin = -(corridorWidth - 1) / 2;
        int offsetMax = corridorWidth / 2;

        if (start.x == end.x)
        {
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = offsetMin; x <= offsetMax; x++)
                    corridorTilemap.SetTile(new Vector3Int(start.x + x, y, 0), corridorTile);
            }
        }
        else
        {
            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = offsetMin; y <= offsetMax; y++)
                    corridorTilemap.SetTile(new Vector3Int(x, start.y + y, 0), corridorTile);
            }
        }
    }
}
