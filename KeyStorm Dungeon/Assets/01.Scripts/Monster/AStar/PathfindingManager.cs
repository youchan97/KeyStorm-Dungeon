using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : SingletonManager<PathfindingManager>
{
    private Grid grid;
    public Grid Grid => grid;
    private Pathfinding pathfinding;

    protected override void Awake()
    {
        base.Awake();
    }

    public Pathfinding CreateRoomPathfinding(Tilemap baseGroundmap, Tilemap groundObstacleMap, Tilemap airObstacleMap, Tilemap universalObstacleMap)
    {
        if (baseGroundmap == null) return null;

        Grid roomGrid = new Grid(baseGroundmap, groundObstacleMap, airObstacleMap, universalObstacleMap);
        Pathfinding roomPathfinding = new Pathfinding(roomGrid);

        return roomPathfinding;
    }

    public List<Node> RequestPath(Vector3 startWorldPos, Vector3 targetWorldPos, UnitType unitType)
    {
        if (grid == null || pathfinding == null)
        {
            //Debug.LogError("PathfindingManager가 초기화되지 않음. 맵 생성 완료 이후 InitializeOrUpdateGrid를 호출 바람.");
            return null;
        }
        return pathfinding.FindPath(startWorldPos, targetWorldPos, unitType);
    }
}
