using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    [SerializeField] private int maxSlots = 24;

    private readonly List<ItemData> _items = new();
    public IReadOnlyList<ItemData> Items => _items;

    public event Action<IReadOnlyList<ItemData>> OnChanged;

    public void Add(ItemData item)
    {
        if (item == null) return;

        if (_items.Count >= maxSlots)
            _items.RemoveAt(_items.Count - 1);

        _items.Insert(0, item); // 최신이 0번(좌상단)
        OnChanged?.Invoke(_items);
    }
}
