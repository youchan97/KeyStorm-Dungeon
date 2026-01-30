using UnityEngine.Tilemaps;

public class PathfindingManager : SingletonManager<PathfindingManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    // 방의 Pathfinding 객체 생성
    public Pathfinding CreateRoomPathfinding(Tilemap baseGroundmap, Tilemap groundObstacleMap, Tilemap airObstacleMap, Tilemap universalObstacleMap)
    {
        if (baseGroundmap == null) return null;

        Grid roomGrid = new Grid(baseGroundmap, groundObstacleMap, airObstacleMap, universalObstacleMap);
        Pathfinding roomPathfinding = new Pathfinding(roomGrid);

        return roomPathfinding;
    }
}