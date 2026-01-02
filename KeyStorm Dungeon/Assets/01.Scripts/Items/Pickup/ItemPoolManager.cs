using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
    public static ItemPoolManager Instance { get; private set; }

    private readonly HashSet<string> spawnedItemIds = new HashSet<string>();

    [Header("티어별 확률")]
    public float tier0Weight = 3f;
    public float tier1Weight = 3f;
    public float tier2Weight = 3f;
    public float tier3Weight = 2f;
    public float tier4Weight = 1f;

    private readonly HashSet<string> acquiredItemIds = new HashSet<string>();

    private readonly Dictionary<ItemDropRoom, Dictionary<ItemTier, List<ItemData>>> pools
        = new Dictionary<ItemDropRoom, Dictionary<ItemTier, List<ItemData>>>();

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

        foreach (ItemDropRoom room in Enum.GetValues(typeof(ItemDropRoom)))
        {
            if (room == ItemDropRoom.None) continue;

            pools[room] = new Dictionary<ItemTier, List<ItemData>>();
            foreach (ItemTier tier in Enum.GetValues(typeof(ItemTier)))
            {
                pools[room][tier] = new List<ItemData>();
            }
        }

        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        var rooms = new List<ItemDropRoom>(pools.Keys);

        foreach (var item in allItems)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.itemId)) continue;

            foreach (var room in rooms)
            {
                if ((item.dropRoom & room) != 0)
                {
                    if (!pools[room].ContainsKey(item.tier))
                        continue;

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
        return GetRandomFiltered(room, requireActive: null);
    }

    public ItemData GetRandomPassive_ExcludeAcquired(ItemDropRoom room)
    {
        return GetRandomFiltered(room, requireActive: false);
    }

    public ItemData GetRandomActive_ExcludeAcquired(ItemDropRoom room)
    {
        return GetRandomFiltered(room, requireActive: true);
    }
    private ItemData GetRandomFiltered(ItemDropRoom room, bool? requireActive)
    {
        if (room == ItemDropRoom.None) return null;
        if (!pools.ContainsKey(room)) return null;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            ItemTier tier = GetRandomTier();
            var list = pools[room][tier];
            if (list == null || list.Count == 0) continue;

            List<ItemData> candidates = null;

            foreach (var it in list)
            {
                if (it == null) continue;

                if (spawnedItemIds.Contains(it.itemId)) continue;
                if (IsAcquired(it.itemId)) continue;

                if (requireActive.HasValue && it.isActiveItem != requireActive.Value)
                    continue;

                candidates ??= new List<ItemData>();
                candidates.Add(it);
            }

            if (candidates == null || candidates.Count == 0) continue;

            ItemData picked = candidates[UnityEngine.Random.Range(0, candidates.Count)];

            spawnedItemIds.Add(picked.itemId);

            return picked;
        }

        return null;
    }

    private ItemTier GetRandomTier()
    {
        float total = tier0Weight + tier1Weight + tier2Weight + tier3Weight + tier4Weight;
        float v = UnityEngine.Random.Range(0f, total);

        if (v < tier0Weight) return ItemTier.Tier0; v -= tier0Weight;
        if (v < tier1Weight) return ItemTier.Tier1; v -= tier1Weight;
        if (v < tier2Weight) return ItemTier.Tier2; v -= tier2Weight;
        if (v < tier3Weight) return ItemTier.Tier3;
        return ItemTier.Tier4;
    }
}
