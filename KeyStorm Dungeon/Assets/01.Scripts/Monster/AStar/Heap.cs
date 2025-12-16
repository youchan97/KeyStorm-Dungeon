using System;

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;              // 힙에 저장될 노드 배열
    private int currentItemCount;   // 현재 힙에 들어있는 노드 개수

    // 최대 힙 사이즈 (그리드의 총 노드 개수)
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// 힙에 새 노드 추가
    /// </summary>
    /// <param name="item">노드</param>
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;  // 노드 힙 인덱스 기록
        items[currentItemCount] = item;     // 배열의 끝에 노드 추가
        SortUp(item);                       // 노드 비교 후 정렬
        currentItemCount++;                 // 노드 개수 증가
    }

    // 힙에서 우선순위가 높은 값 제거 및 반환
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    // 노드 값이 변경되었을 때 힙 재정렬
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    // 힙에 특정 노드가 포함되어 있는지 확인
    public bool Contains(T item)
    {
        // 힙 인덱스가 유효한 범위 내에 있는가,
        // 해당 인덱스의 노드가 실제 노드와 동일한가
        if (item.HeapIndex < 0 || item.HeapIndex >= currentItemCount)
        {
            return false;
        }

        return items[item.HeapIndex].Equals(item);
    }

    // 현재 힙에 들어있는 노드의 개수
    public int Count
    {
        get { return currentItemCount; }
    }

    // 노드를 아래로 정렬
    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexRight].CompareTo(items[childIndexLeft]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    // 노드를 위로 정렬
    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];

            if(item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    // 두 노드의 배열 내 위치를 교환
    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}
