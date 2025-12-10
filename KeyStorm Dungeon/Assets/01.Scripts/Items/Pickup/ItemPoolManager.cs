using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
    public static ItemPoolManager Instance { get; private set; }

    [Header("티어별 확률")]
    [Tooltip("Tier 0~4 순서대로 확률. 합이 1이 아니어도 되고, 비율만 맞으면 됨.")]
    public float tier0Weight = 3f; // 3/12
    public float tier1Weight = 3f; // 3/12
    public float tier2Weight = 3f; // 3/12
    public float tier3Weight = 2f; // 2/12
    public float tier4Weight = 1f; // 1/12

    // DropRoom + Tier 조합별 아이템 리스트
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

    void BuildPools()
    {
        pools.Clear();

        // DropRoom 종류별로 초기화
        foreach (ItemDropRoom room in System.Enum.GetValues(typeof(ItemDropRoom)))
        {
            pools[room] = new Dictionary<ItemTier, List<ItemData>>();
            foreach (ItemTier tier in System.Enum.GetValues(typeof(ItemTier)))
            {
                pools[room][tier] = new List<ItemData>();
            }
        }

        // ItemDatabase에서 모든 아이템 가져와서 풀에 넣기 ItemDatabase가 먼저 씬에 존재해야 함
        foreach (ItemDropRoom room in System.Enum.GetValues(typeof(ItemDropRoom)))
        {
            foreach (ItemTier tier in System.Enum.GetValues(typeof(ItemTier)))
            {
                // Resources에서 직접 불러오기보다, ItemDatabase의 itemDict를 순회하는 방식이 더 좋지만 여기서는 간단하게 Resources.LoadAll을 한 번 더 사용할 수도 있음
            }
        }

        // 간단한 버전: Resources에서 한 번 더 로드해서 분류
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItems)
        {
            pools[item.dropRoom][item.tier].Add(item);
        }
    }

    public ItemData GetRandomItem(ItemDropRoom dropRoom)
    {
        // 1) 먼저 티어를 하나 뽑는다 (티어 확률표 기반)
        ItemTier chosenTier = GetRandomTier();

        // 2) 해당 DropRoom + Tier 조합에서 아이템 리스트를 가져온다
        List<ItemData> list = pools[dropRoom][chosenTier];

        // 만약 해당 티어에 아이템이 하나도 없다면, 다른 티어를 시도하거나 null 반환
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning($"[ItemPoolManager] {dropRoom} / {chosenTier} 에 아이템이 없음");
            return null;
        }

        // 3) 리스트에서 랜덤 1개 선택
        int index = Random.Range(0, list.Count);
        return list[index];
    }

    ItemTier GetRandomTier()
    {
        float total = tier0Weight + tier1Weight + tier2Weight + tier3Weight + tier4Weight;
        float value = Random.Range(0f, total);

        if (value < tier0Weight) return ItemTier.Tier0;
        value -= tier0Weight;

        if (value < tier1Weight) return ItemTier.Tier1;
        value -= tier1Weight;

        if (value < tier2Weight) return ItemTier.Tier2;
        value -= tier2Weight;

        if (value < tier3Weight) return ItemTier.Tier3;
        value -= tier3Weight;

        return ItemTier.Tier4;
    }
}
