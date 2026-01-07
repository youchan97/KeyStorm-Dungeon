using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ConstValue;
public class StageManager : MonoBehaviour
{
    private readonly int distanceWeight = 2;

    StageDataManager stageDataManager;

    [Header("Stage Data")]
    public StageData stageData;

    [Header("PlayerSpawn")]
    public PlayerSpawner playerSpawner;

    [Header("Corridor Tilemap")]
    public Tilemap corridorTilemap;
    public TileBase horizontalCorridorTile;
    public TileBase verticalCorridorTile;

    [Header("Config")]
    public int roomSpacing = 20;
    public int corridorWidth;
    public float expandWeight; //확장 가중치(랜덤으로 뻗어나가기 위함) 낮을 수록 균등하게 분포될 확률이 증가

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
        stageDataManager = StageDataManager.Instance;
        stageData = stageDataManager.CurrentStageData;

        AudioManager.Instance.PlayBgm(BgmSetting());

        GenerateRoomLayout();
        SpawnRooms();
        ConnectRooms();
    }

    string BgmSetting()
    {
        switch(stageDataManager.CurrentDifficulty)
        {
            case StageDifficulty.Easy:
                return EasyBgm;
            case StageDifficulty.Normal:
                return NormalBgm;
            case StageDifficulty.Hard:
                return HardBgm;
            default:
                return EasyBgm;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            GameManager.Instance.StageClear();
        }
    }

    #region 스테이지 생성
    void GenerateRoomLayout()
    {
        roomMap.Clear();

        Vector2Int start = Vector2Int.zero;
        roomMap[start] = new RoomNode
        {
            gridPos = start,
            type = RoomType.Start
        };

        List<Vector2Int> expandList = new List<Vector2Int>();
        expandList.Add(start);

        while (roomMap.Count < stageData.totalRoomCount && expandList.Count > 0)
        {
            Vector2Int current = expandList[Random.Range(0, expandList.Count)];

            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];
            Vector2Int next = current + dir;

            if (roomMap.ContainsKey(next))
                continue;

            roomMap[next] = new RoomNode
            {
                gridPos = next,
                type = RoomType.Normal
            };

            // 새 방은 확장 후보
            expandList.Add(next);

            // 현재 방도 다시 확장 가능 (이게 깊이를 만듦)
            if (Random.value < expandWeight)
                expandList.Add(current);

            // 너무 막힌 방은 제거 (선택)
            if (GetNeighborCount(current) >= dirs.Length)
                expandList.Remove(current);
        }
    }

    int GetNeighborCount(Vector2Int pos)
    {
        int count = 0;
        foreach (var dir in dirs)
        {
            if (roomMap.ContainsKey(pos + dir))
                count++;
        }
        return count;
    }

    int GetBossRouteWeight(RoomNode node)
    {
        int distance = Mathf.Abs(node.gridPos.x) + Mathf.Abs(node.gridPos.y);
        int neighbor = GetNeighborCount(node.gridPos);

        return distance * distanceWeight - neighbor;
    }

    List<RoomNode> GetBossRoomNodes()
    {
        List<(RoomNode node, int score)> bossRoomList = new List<(RoomNode node, int score)>();

        foreach (RoomNode node in roomMap.Values)
        {
            if (node.type == RoomType.Start)
                continue;

            int score = GetBossRouteWeight(node);
            bossRoomList.Add((node, score));
        }

        bossRoomList.Sort((a, b) => b.score.CompareTo(a.score));

        List<RoomNode> boosRoomNodes = new List<RoomNode>();
        for (int i = 0; i < stageData.bossRoomCount && i < bossRoomList.Count; i++)
        {
            boosRoomNodes.Add(bossRoomList[i].node);
        }

        return boosRoomNodes;
    }

    void SpawnRooms()
    {
        spawnedRooms.Clear();

        List<RoomNode> bossNodes = GetBossRoomNodes();

        int startRoom = 1;
        int fixTotal = stageData.bossRoomCount + startRoom;

        int specialTotal = stageData.treasureRoomCount + stageData.shopRoomCount;

        if (specialTotal + fixTotal > roomMap.Count) return;

        List<Room> spawnPool = new List<Room>();

        spawnPool.AddRange(PickRandom(stageDataManager.CurrentStageSet.treasureRooms, stageData.treasureRoomCount));
        spawnPool.AddRange(PickRandom(stageDataManager.CurrentStageSet.shopRooms, stageData.shopRoomCount));

        int normalCount = roomMap.Count - spawnPool.Count - fixTotal;
        spawnPool.AddRange(PickRandom(stageDataManager.CurrentStageSet.normalRooms, normalCount));

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
                    curRoom = stageDataManager.CurrentStageSet.startRoom;
                    break;

                default:
                    if(bossNodes.Contains(node))
                        curRoom = stageDataManager.CurrentStageSet.bossRooms[Random.Range(0, stageDataManager.CurrentStageSet.bossRooms.Count)];
                    else
                        curRoom = spawnPool[i++];
                    break;
            }

            Room room = Instantiate(curRoom, worldPos, Quaternion.identity);
            spawnedRooms[node.gridPos] = room;
        }
        playerSpawner.SpawnPlayer();
    }

    List<Room> PickRandom(List<Room> source, int count)
    {
        return source.OrderBy(_ => Random.value).Take(count).ToList();
    }

    void ConnectRooms()
    {
        foreach (var spawnedRoom in spawnedRooms)
        {
            Vector2Int pos = spawnedRoom.Key;
            Room room = spawnedRoom.Value;

            foreach (var dir in dirs)
            {
                Vector2Int next = pos + dir;
                if (!spawnedRooms.ContainsKey(next)) continue;

                if (pos.x > next.x || pos.y > next.y) continue;

                Room other = spawnedRooms[next];

                Transform start = room.GetDoor(dir).transform;
                Transform end = other.GetDoor(-dir).transform;

                start.GetComponent<Door>()?.UseDoor();
                end.GetComponent<Door>()?.UseDoor();

                DrawCorridor(start.position, end.position);
            }
        }
    }

    void DrawCorridor(Vector3 from, Vector3 to)
    {
        Vector3Int start = corridorTilemap.WorldToCell(from);
        Vector3Int end = corridorTilemap.WorldToCell(to);

        Vector3Int dir = end - start;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.y = Mathf.Clamp(dir.y, -1, 1);

        end -= dir;

        int offsetMin = -(corridorWidth - 1) / 2;
        int offsetMax = corridorWidth / 2;

        if (start.x == end.x)
        {
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = offsetMin; x <= offsetMax; x++)
                    corridorTilemap.SetTile(new Vector3Int(start.x + x, y, 0), stageDataManager.CurrentStageSet.verticalCorridor);
            }
        }
        else
        {
            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = offsetMin; y <= offsetMax; y++)
                    corridorTilemap.SetTile(new Vector3Int(x, start.y + y, 0), stageDataManager.CurrentStageSet.horizontalCorridor);
            }
        }
    }
    #endregion
}
