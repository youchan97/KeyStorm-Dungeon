using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private Grid grid;
    public Grid Grid => grid;

    // Grid 초기화
    public Pathfinding(Grid grid)
    {
        this.grid = grid;
    }

    /// <summary>
    /// a*를 사용해 특정 시작점에서 목표점까지 최적 경로를 탐색
    /// </summary>
    /// <param name="startWorldPos">시작 지점의 월드 좌표</param>
    /// <param name="targetWorldPos">목표 지점의 월드 좌표</param>
    /// <param name="unitType">지상 유닛인지 공중 유닛인지 구분</param>
    /// <returns>최단 경로를 나타내는 Node 리스트(없으면 null)</returns>
    public List<Node> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos, UnitType unitType)
    {
        // 비용 및 부모노드 초기화
        for (int x = 0; x < grid.gridSize.x; x++)
        {
            for (int y = 0; y < grid.gridSize.y; y++)
            {
                var node = grid.nodes[x, y];
                node.gCost = int.MaxValue;
                node.hCost = 0;
                node.parent = null;
            }
        }

        // 시작과 목표 노드 변환
        Node startNode = grid.GetNodeFromWorldPoint(startWorldPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetWorldPos);

        // 시작 또는 목표 위치가 그리드 밖이거나 이동 불가하다면 탐색 종료
        if (startNode == null || targetNode == null)
        {
            return null;
        }

        // 이동 가능 여부 판단 (지상/공중 유닛 구분)
        bool startWalkable = (unitType == UnitType.Ground) ? startNode.isWalkableGround : startNode.isWalkableAir;
        bool targetWalkable = (unitType == UnitType.Ground) ? targetNode.isWalkableGround : targetNode.isWalkableAir;

        if (!startWalkable || !targetWalkable)
        {
            return null;
        }

        // 오픈 리스트(미확인 노드)를 힙 형태로 생성
        Heap<Node> openSet = new Heap<Node>(grid.gridSize.x * grid.gridSize.y);
        HashSet<Node> closedSet = new HashSet<Node>();  // 방문을 완료한 노드들의 집합

        // 시작 노드 초기 비용 세팅 이후 오픈 리스트에 추가
        startNode.gCost = 0;
        startNode.hCost = GetHeuristicCost(startNode, targetNode);
        startNode.parent = null;
        openSet.Add(startNode);

        // 경로 탐색 반복
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();   // 최우선 노드 추출
            closedSet.Add(currentNode);                 // 방문

            // 목표 노드 도달 시 경로 반환
            if (currentNode == targetNode)
            {
                List<Node> path = ReconstructPath(startNode, targetNode);
                return path;
            }

            // 주변 이웃 노드 조회
            foreach(Node neighbor in grid.GetNeighbors(currentNode, unitType))
            {
                bool neighborWalkable = (unitType == UnitType.Ground) ? neighbor.isWalkableGround : neighbor.isWalkableAir;

                // 장애물이 있거나 이미 방문한 노드면 다음
                if(!neighborWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                // 현재 노드를 통해 이웃 노드로 가는 예상 비용 계산
                int newMovementCostToNeighbor = currentNode.gCost + GetMovementCost(currentNode, neighbor, unitType);

                // 비용이 더 낮거나 아직 오픈 리스트에 없는 경우 업데이트
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetHeuristicCost(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbor);
                    }
                }
            }
        }
        // 경로 탐색 실패 시 null 반환
        return null;
    }

    // 목표 지점부터 시작 지점까지 부모 노드를 따라 경로를 역추적하여 리스트 생성
    private List<Node> ReconstructPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;

            if(currentNode == null)
            {
                return null;
            }
        }

        path.Reverse();
        return path;
    }

    // 이동 비용 계산 (가중치 포함)
    private int GetMovementCost(Node nodeA, Node nodeB, UnitType unitType)
    {
        int distanceX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int distanceY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        int baseCost;

        if (distanceX > distanceY)
        {
            baseCost = 14 * distanceY + 10 * (distanceX - distanceY);
        }
        else
        {
            baseCost = 14 * distanceX + 10 * (distanceY - distanceX);
        }

        int penalty = 0;
        foreach (var neighbor in grid.GetNeighbors(nodeB, unitType))
        {
            bool isBlocked = (unitType == UnitType.Ground) ? !neighbor.isWalkableGround : !neighbor.isWalkableAir;

            if (isBlocked)
            {
                penalty += 10;
            }
        }

        return baseCost + penalty;
    }

    // 휴리스틱 비용 계산 (유클리드 거리)
    private int GetHeuristicCost(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int distanceY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        /*return 10 * (distanceX + distanceY);*/
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);
        return Mathf.RoundToInt(distance * 10);
    }
}
