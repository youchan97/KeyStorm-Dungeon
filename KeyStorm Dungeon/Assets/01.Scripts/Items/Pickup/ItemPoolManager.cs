using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
    public static ItemPoolManager Instance { get; private set; }

    [Header("티어별 확률")]
    public float tier0Weight = 3f;
    public float tier1Weight = 3f;
    public float tier2Weight = 3f;
    public float tier3Weight = 2f;
    public float tier4Weight = 1f;

    // 먹은 아이템(다시 안 나오게 학)
    private readonly HashSet<string> acquiredItemIds = new HashSet<string>();

    private Dictionary<ItemDropRoom, Dictionary<ItemTier, List<ItemData>>> pools = new Dictionary<ItemDropRoom, Dictionary<ItemTier, List<ItemData>>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildPools();
    }

    public void BuildPools()
    {
        pools.Clear();

        // 드랍룸 / 티어별 리스트 초기화
        foreach (ItemDropRoom room in System.Enum.GetValues(typeof(ItemDropRoom)))
        {
            // Flags면 None도 들어올 수 있으니, None은 풀에서 제외 추천
            if (room == ItemDropRoom.None) continue;

            pools[room] = new Dictionary<ItemTier, List<ItemData>>();
            foreach (ItemTier tier in System.Enum.GetValues(typeof(ItemTier)))
            {
                pools[room][tier] = new List<ItemData>();
            }
        }

        // 모든 아이템데이터 로드해서 분류
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItems)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.itemId)) continue;

            // DropRoom이 Flags인 경우: 여러 방에 들어갈 수 있음
            foreach (var room in pools.Keys)
            {
                if ((item.dropRoom & room) != 0)
                {
                    pools[room][item.tier].Add(item);
                }
            }
        }
    }

    public bool IsAcquired(string itemId)
        => !string.IsNullOrEmpty(itemId) && acquiredItemIds.Contains(itemId);

    public void MarkAcquired(ItemData data)
    {
        if (data == null) return;
        if (string.IsNullOrEmpty(data.itemId)) return;
        acquiredItemIds.Add(data.itemId);
    }

    public ItemData GetRandomItem_ExcludeAcquired(ItemDropRoom room)
    {
        if (room == ItemDropRoom.None) return null;
        if (!pools.ContainsKey(room)) return null;

        ItemTier tier = GetRandomTier();
        var list = pools[room][tier];
        if (list == null || list.Count == 0) return null;

        List<ItemData> candidates = null;
        foreach (var it in list)
        {
            if (it == null) continue;
            if (IsAcquired(it.itemId)) continue;

            candidates ??= new List<ItemData>();
            candidates.Add(it);
        }

        if (candidates == null || candidates.Count == 0) return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private ItemTier GetRandomTier()
    {
        float total = tier0Weight + tier1Weight + tier2Weight + tier3Weight + tier4Weight;
        float v = Random.Range(0f, total);

        if (v < tier0Weight) return ItemTier.Tier0; v -= tier0Weight;
        if (v < tier1Weight) return ItemTier.Tier1; v -= tier1Weight;
        if (v < tier2Weight) return ItemTier.Tier2; v -= tier2Weight;
        if (v < tier3Weight) return ItemTier.Tier3;
        return ItemTier.Tier4;
    }
}
