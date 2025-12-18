using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : SingletonManager<PathfindingManager>
{
    [Header("타일맵 할당")]
    [SerializeField] private Tilemap baseGroundTilemap;
    [SerializeField] private Tilemap groundObstacleTilemap;
    [SerializeField] private Tilemap airObstacleTilemap;
    [SerializeField] private Tilemap universalObstacleTilemap;

    [Header("디버깅 시각화")]
    public bool drawGizmos = true;
    public Vector3 gizmoDrawOffset = new Vector3(0, 0.1f, 0);

    private Grid grid;
    public Grid Grid => grid;
    private Pathfinding pathfinding;

    protected override void Awake()
    {
        base.Awake();

        InitializePathfinding();
    }

    // 임시 사용 고정맵 전용
    private void InitializePathfinding()
    {
        if (baseGroundTilemap == null)
        {
            Debug.LogError("PathfindingManager : BaseGroundTilemap이 할당되지 않아 Grid를 초기화할 수 없음.");
            return;
        }

        grid = new Grid(baseGroundTilemap, groundObstacleTilemap, airObstacleTilemap, universalObstacleTilemap);
        pathfinding = new Pathfinding(grid);
    }

    public void InitializeOrUpdateGrid(Tilemap baseGroundmap, Tilemap groundObstacleMap,  Tilemap airObstacleMap, Tilemap universalObstacleMap)
    {
        if (baseGroundmap == null)
        {
            Debug.LogError("PathfindingManager : BaseGroundTilemap이 할당되지 않아 Grid를 초기화할 수 없음.");
            return;
        }

        grid = new Grid(baseGroundmap, groundObstacleMap, airObstacleMap, universalObstacleMap);
        pathfinding = new Pathfinding(grid);
    }

    public List<Node> RequestPath(Vector3 startWorldPos, Vector3 targetWorldPos, UnitType unitType)
    {
        if (grid == null || pathfinding == null)
        {
            Debug.LogError("PathfindingManager가 초기화되지 않음. 맵 생성 완료 이후 InitializeOrUpdateGrid를 호출 바람.");
            return null;
        }
        return pathfinding.FindPath(startWorldPos, targetWorldPos, unitType);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || grid == null || baseGroundTilemap == null) return;

        for (int x = 0; x < grid.gridSize.x; x++)
        {
            for (int y = 0; y < grid.gridSize.y; y++)
            {
                Node n = grid.nodes[x, y];
                Vector3 worldPos = grid.GetWorldPointFromNode(n);
                Color baseColor = Color.gray;

                if (n.isWalkableGround && n.isWalkableAir)
                {
                    baseColor = Color.white;
                }
                else if(n.isWalkableGround && !n.isWalkableAir)
                {
                    baseColor = Color.blue;
                }
                else if(!n.isWalkableGround && n.isWalkableAir)
                {
                    baseColor = Color.cyan;
                }
                else
                {
                    baseColor = Color.red;
                }
                Gizmos.color = baseColor;
                Gizmos.DrawCube(worldPos + gizmoDrawOffset, Vector3.one * baseGroundTilemap.cellSize.x * 0.5f);
            }
        }

        if (pathfinding != null && pathfinding.currentDebugPath != null && pathfinding.currentDebugPath.Count > 0)
        {
            Gizmos.color = Color.green;
            
            for (int i = 0; i < pathfinding.currentDebugPath.Count; i++)
            {
                Vector3 from = grid.GetWorldPointFromNode(pathfinding.currentDebugPath[i]) + gizmoDrawOffset;
                
                if(i < pathfinding.currentDebugPath.Count - 1)
                {
                    Vector3 to = grid.GetWorldPointFromNode(pathfinding.currentDebugPath[i + 1]) + gizmoDrawOffset;
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawSphere(from, baseGroundTilemap.cellSize.x * 0.1f);
                }
                else
                {
                    Gizmos.DrawSphere(from, baseGroundTilemap.cellSize.x * 0.2f);
                }
            }
        }
    }
}
