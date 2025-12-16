using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    // 최신 획득이 0번(왼쪽 위)
    public readonly List<ItemData> items = new();

    public void AddItem(ItemData data)
    {
        if (data == null) return;
        items.Insert(0, data);
    }
}
