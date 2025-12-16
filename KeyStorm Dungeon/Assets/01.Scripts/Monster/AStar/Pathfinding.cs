using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pathfinding
{
    private Grid grid;
    public List<Node> currentDebugPath;

    public Pathfinding(Grid grid)
    {
        this.grid = grid;
    }

    public List<Node> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos, UnitType unitType)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startWorldPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetWorldPos);

        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning($"Pathfinding : 시작점 {startWorldPos}, 목표점 {targetWorldPos} 노드가 없음)");
            currentDebugPath = null;
            return null;
        }

        bool startWalkable = (unitType == UnitType.Ground) ? startNode.isWalkableGround : startNode.isWalkableAir;
        bool targetWalkable = (unitType == UnitType.Ground) ? targetNode.isWalkableGround : targetNode.isWalkableAir;

        if (!startWalkable || !targetWalkable)
        {
            Debug.LogWarning($"Pathfinding {unitType} : 시적점 또는 목표점에 접근할 수 없음" +
                $"시작 노드 {startNode.gridPos}: {startWalkable}, 목표 노드 {targetNode.gridPos}: {targetWalkable}");
            currentDebugPath = null;
            return null;
        }

        Heap<Node> openSet = new Heap<Node>(grid.gridSize.x * grid.gridSize.y);
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        startNode.parent = null;
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                List<Node> path = ReconstructPath(startNode, targetNode);
                currentDebugPath = path;
                return path;
            }

            foreach(Node neighbor in grid.GetNeighbors(currentNode))
            {
                bool neighborWalkable = (unitType == UnitType.Ground) ? neighbor.isWalkableGround : neighbor.isWalkableAir;

                if(!neighborWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
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

        Debug.LogWarning($"Pathfinding {unitType}: 경로를 찾을 수 없음");
        currentDebugPath = null;
        return null;
    }

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
                Debug.LogError("Path Reconstruction : 경로가 startNode에 도달하기 전에 parent가 null이 됨");
                return null;
            }
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int distanceY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);
        return Mathf.RoundToInt(distance * 10);

        /*if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }

        return 14 * distanceX + 10 * (distanceY - distanceX);*/
    }
}
