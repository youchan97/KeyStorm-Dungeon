using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid
{
    public Node[,] nodes;       // 모든 노드를 저장하는 2차원 배열
    public Vector3Int gridSize; // 그리드의 전체 크기 (width, height)

    // Tilemap 레이어에 대한 참조
    private Tilemap baseGroundTilemap;  // 기본 지형
    private Tilemap groundObstacleTilemap;  // 지상 유닛을 막는 장애물
    private Tilemap airObstacleTilemap;     // 공중 유닛을 막는 장애물
    private Tilemap universalObstacleTilemap;// 모든 유닛을 막는 장애물

    private Vector3Int origin;

    public Grid(Tilemap baseGroundMap, Tilemap groundObstacleMap, Tilemap airObstacleMap, Tilemap universalObstacleMap)
    {
        this.baseGroundTilemap = baseGroundMap;
        this.groundObstacleTilemap = groundObstacleMap;
        this.airObstacleTilemap = airObstacleMap;
        this.universalObstacleTilemap = universalObstacleMap;

        BoundsInt bounds = baseGroundMap.cellBounds;
        gridSize = new Vector3Int(bounds.size.x, bounds.size.y, 1);
        origin = bounds.min;

        nodes = new Node[gridSize.x, gridSize.y];

        // Node 객체 생성 및 isWalkable 설정
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(origin.x + x, origin.y + y, 0);

                bool hasBaseGround = (baseGroundTilemap.GetTile(cellPos) != null);

                if (!hasBaseGround)
                {
                    nodes[x, y] = new Node(new Vector3Int(x, y, 0), false, false);
                    continue;
                }

                bool hasGroundObstacle = (groundObstacleTilemap != null && groundObstacleMap.GetTile(cellPos) != null);
                bool hasAirObstacle = (airObstacleTilemap != null && airObstacleTilemap.GetTile(cellPos) != null);
                bool hasUnivesalObstacle = (universalObstacleTilemap != null && universalObstacleTilemap.GetTile(cellPos) != null);

                bool isWalkableGround = !hasGroundObstacle && !hasUnivesalObstacle;
                bool isWalkableAir = !hasAirObstacle && !hasUnivesalObstacle;

                nodes[x, y] = new Node(new Vector3Int(x, y, 0), isWalkableGround, isWalkableAir);
            }
        }
    }

    // 월드 좌표를 그리드의 Node 객체로 변환
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        if (baseGroundTilemap == null) return null;

        Vector3Int cellPos = baseGroundTilemap.WorldToCell(worldPosition);

        int x = cellPos.x - origin.x;
        int y = cellPos.y - origin.y;

        if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
        {
            return nodes[x, y];
        }
        return null;
    }

    // Node 객체의 그리드 인덱스를 실제 월드 좌표로 변환
    public Vector3 GetWorldPointFromNode(Node node)
    {
        if (baseGroundTilemap == null || node == null) return Vector3.zero;

        Vector3Int cellPos = new Vector3Int(origin.x + node.gridPos.x, origin.y + node.gridPos.y, 0);

        return baseGroundTilemap.GetCellCenterWorld(cellPos);
    }

    // 특정 노드의 이웃 노드들을 List<Node> 형태로 반환
    // 8방향의 이웃 노드들을 찾음
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // 자기 자신은 이웃이 아니므로

                int checkX = node.gridPos.x + x;
                int checkY = node.gridPos.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbors.Add(nodes[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
}
