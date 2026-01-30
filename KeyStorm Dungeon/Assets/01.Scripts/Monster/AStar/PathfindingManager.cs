using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : SingletonManager<PathfindingManager>
{
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
}
