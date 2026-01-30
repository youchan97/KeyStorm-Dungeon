using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Vector3Int gridPos;  // 그리드 상의 x, y 좌표

    public bool isWalkableGround;   // 지상 유닛이 지나갈 수 있는가 = 추가 고려용
    public bool isWalkableAir;      // 공중 유닛이 지나갈 수 있는가 = 돌, 구멍 등의 바닥 장애물

    public Node parent;             // 경로 재구성을 위한 부모 노드

    public int gCost;               // 시작 노드로부터 이 노드까지의 실제 비용 (G 비용)
    public int hCost;               // 이 노드로부터 목표 노드까지의 추정 비용 (H 비용)
    public int FCost { get { return gCost + hCost; } }  // 총 비용 (F 비용 = G 비용 + H 비용)

    // A* 알고리즘 내부에서 OpenList를 힙처럼 관리하기 위한 인덱스
    private int heapIndex;
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    public Node(Vector3Int gridPos, bool isWalkableGround, bool isWalkableAir)
    {
        this.gridPos = gridPos;
        this.isWalkableGround = isWalkableGround;
        this.isWalkableAir = isWalkableAir;
    }


    /// <summary>
    /// 두 노드를 비교해 F 비용이 낮은 노드를 우선
    /// F 비용이 같다면 H 비용이 낮은 노드를 우선
    /// </summary>
    /// <param name="nodeToCompare">비교를 위한 노드</param>
    /// <returns></returns>
    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
