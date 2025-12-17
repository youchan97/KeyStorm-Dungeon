using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject root;                 // InventoryUI
    public InventoryModel model;            // InventoryModel
    public Transform gridParent;            // ItemGridPanel
    public InventorySlotUI slotPrefab;      // ItemSlot prefab

    [Header("Grid Settings")]
    public int columns = 5;
    public int capacity = 30;

    private readonly List<InventorySlotUI> slots = new();
    private int selectedIndex = 0;
    private bool opened = false;

    private void Start()
    {
        BuildSlots();
        Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (opened) Close();
            else Open();
        }

        if (!opened) return;

        HandleNavigation();
    }

    private void BuildSlots()
    {
        for (int i = 0; i < capacity; i++)
        {
            var s = Instantiate(slotPrefab, gridParent);
            s.Set(null);
            s.SetSelected(false);
            slots.Add(s);
        }
    }

    private void Open()
    {
        opened = true;
        root.SetActive(true);

        selectedIndex = 0;   // 입장 시 왼쪽 위 선택
        Refresh();
    }

    private void Close()
    {
        opened = false;
        root.SetActive(false);
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            ItemData data = (model.items.Count > i) ? model.items[i] : null;
            slots[i].Set(data);
        }

        UpdateSelection();

        // 오른쪽 아이템 설명 UI 갱신 코드는 없음
    }

    private void HandleNavigation()
    {
        int count = Mathf.Min(model.items.Count, capacity);
        if (count <= 0) return;

        int prev = selectedIndex;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectedIndex = (selectedIndex - 1 + count) % count;
        if (Input.GetKeyDown(KeyCode.RightArrow)) selectedIndex = (selectedIndex + 1) % count;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedIndex = MoveUp(selectedIndex, count, columns);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedIndex = MoveDown(selectedIndex, count, columns);

        if (prev != selectedIndex) UpdateSelection();
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetSelected(false);

        int count = Mathf.Min(model.items.Count, capacity);
        if (count <= 0) return;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, count - 1);
        slots[selectedIndex].SetSelected(true);

        // 여기에도 설명 UI 갱신 없음(placeholder만 존재)
    }

    private int MoveUp(int idx, int count, int cols)
    {
        int target = idx - cols;
        if (target >= 0) return target;

        int col = idx % cols;
        int lastRowStart = ((count - 1) / cols) * cols;
        int candidate = lastRowStart + col;
        while (candidate >= count) candidate--;
        return candidate;
    }

    private int MoveDown(int idx, int count, int cols)
    {
        int target = idx + cols;
        if (target < count) return target;

        int col = idx % cols;
        int candidate = col;
        return (candidate < count) ? candidate : 0;
    }
}
