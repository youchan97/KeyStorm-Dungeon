using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    private int totalGoldEarned = 0;
    private List<AcquiredItemData> acquiredItems = new List<AcquiredItemData>();
    private HashSet<string> addedItemNames = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameDataManager] 생성됨");
        }
        else if (Instance != this)
        {
            Debug.Log("[GameDataManager] 중복 제거");
            Destroy(gameObject);
        }
    }

    public void AddGold(int amount)
    {
        totalGoldEarned += amount;
        Debug.Log($"[GameDataManager] 골드 획득: +{amount}G (총 획득: {totalGoldEarned}G)");
    }

    public int GetTotalGold()
    {
        return totalGoldEarned;
    }

    public void AddItem(AcquiredItemData item)
    {
        if (item == null || item.itemIcon == null)
        {
            Debug.LogWarning("[GameDataManager] 아이템 또는 아이콘이 null입니다!");
            return;
        }

        if (addedItemNames.Contains(item.itemName))
        {
            Debug.Log($"[GameDataManager] 중복 아이템 무시: {item.itemName}");
            return;
        }

        acquiredItems.Add(item);
        addedItemNames.Add(item.itemName);
        Debug.Log($"[GameDataManager] 아이템 추가됨: {item.itemName} (총 {acquiredItems.Count}개)");
    }

    public List<AcquiredItemData> GetAcquiredItems()
    {
        Debug.Log($"[GameDataManager] 아이템 리스트 반환: {acquiredItems.Count}개");
        return new List<AcquiredItemData>(acquiredItems);
    }

    public void ResetAllData()
    {
        totalGoldEarned = 0; 
        acquiredItems.Clear();
        addedItemNames.Clear();

        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.ResetTimer();
        }

        Debug.Log("[GameDataManager] 모든 데이터 초기화됨");
    }
}

[System.Serializable]
public class AcquiredItemData
{
    public string itemName;
    public Sprite itemIcon;

    public AcquiredItemData(string name, Sprite icon)
    {
        itemName = name;
        itemIcon = icon;
    }
}