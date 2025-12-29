using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySelectionController : MonoBehaviour
{
    [SerializeField] private int columns = 6;

    private List<InventorySlotUI> _slots;
    private ItemDescriptionView _desc;
    private int _index = -1;

    public void Init(List<InventorySlotUI> slots, ItemDescriptionView desc)
    {
        _slots = slots;
        _desc = desc;
    }

    public void SelectIndex(int i) => Apply(i);

    public void Clear()
    {
        if (_slots == null) return;
        if (_index >= 0 && _index < _slots.Count) _slots[_index].SetSelected(false);
        _index = -1;
    }

    private void Update()
    {
        if (_slots == null || _slots.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(-1, 0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(1, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move(0, -1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(0, 1);
    }

    private void Move(int dx, int dy)
    {
        if (_index < 0) _index = 0;

        int total = _slots.Count;
        int rows = Mathf.CeilToInt((float)total / columns);

        int x = _index % columns;
        int y = _index / columns;

        if (dx != 0)
        {
            x += dx;
            if (x >= columns) { x = 0; y = (y + 1) % rows; }
            if (x < 0) { x = columns - 1; y = (y - 1 + rows) % rows; }
        }

        if (dy != 0)
        {
            y += dy;
            if (y < 0) y = rows - 1;
            if (y >= rows) y = 0;
        }

        int next = y * columns + x;
        if (next >= total) next = total - 1;

        Apply(next);
    }

    private void Apply(int next)
    {
        if (_slots == null || _slots.Count == 0) return;

        next = Mathf.Clamp(next, 0, _slots.Count - 1);

        if (_index >= 0) _slots[_index].SetSelected(false);
        _index = next;
        _slots[_index].SetSelected(true);

        if (_desc != null) _desc.SetItem(_slots[_index].Data);
    }
}
