using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    // 모든 아이템 데이터 (itemId 기준으로 접근)
    private Dictionary<string, ItemData> itemDict = new Dictionary<string, ItemData>();

    [Header("액티브 아이템 픽업 프리팹")]
    [Tooltip("itemId(A1, A2 등)와 해당 액티브 픽업 프리팹을 묶어서 관리")]
    public List<ActivePickupEntry> activePickupEntries = new List<ActivePickupEntry>();

    private Dictionary<string, GameObject> activePickupDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllItems();
        BuildActivePickupDict();
    }

    void LoadAllItems()
    {
        // Resources/Items 폴더에서 모든 아이템데이터 불러오기
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        itemDict.Clear();

        foreach (var item in allItems)
        {
            if (string.IsNullOrEmpty(item.itemId))
            {
                Debug.LogWarning($"ItemData {item.name} 의 itemId가 비어 있음");
                continue;
            }

            if (!itemDict.ContainsKey(item.itemId))
            {
                itemDict.Add(item.itemId, item);
            }
            else
            {
                Debug.LogWarning($"중복된 itemId 발견: {item.itemId} ( {item.name} )");
            }
        }
    }

    void BuildActivePickupDict()
    {
        activePickupDict.Clear();
        foreach (var entry in activePickupEntries)
        {
            if (entry.pickupPrefab == null) continue;
            if (string.IsNullOrEmpty(entry.itemId)) continue;

            if (!activePickupDict.ContainsKey(entry.itemId))
            {
                activePickupDict.Add(entry.itemId, entry.pickupPrefab);
            }
            else
            {
                Debug.LogWarning($"중복된 액티브 픽업 itemId: {entry.itemId}");
            }
        }
    }

    public ItemData GetItemById(string id)
    {
        if (itemDict.TryGetValue(id, out var data))
        {
            return data;
        }
        return null;
    }

    public GameObject GetActivePickupPrefab(string itemId)
    {
        if (activePickupDict.TryGetValue(itemId, out var prefab))
        {
            return prefab;
        }
        return null;
    }
}

[System.Serializable]
public class ActivePickupEntry
{
    public string itemId;          // A1
    public GameObject pickupPrefab; // ActiveItemPickup이 붙은 프리팹
}
