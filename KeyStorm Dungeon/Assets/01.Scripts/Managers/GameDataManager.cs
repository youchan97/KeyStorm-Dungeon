using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    private int totalGold = 0;
    private List<AcquiredItemData> acquiredItems = new List<AcquiredItemData>();
    private HashSet<string> addedItemNames = new HashSet<string>(); 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetTotalGold(int gold)
    {
        totalGold = gold;
    }

    public void AddGold(int amount)
    {
        totalGold += amount;
    }

    public int GetTotalGold()
    {
        return totalGold;
    }

    public void AddItem(AcquiredItemData item)
    {
        if (item == null || item.itemIcon == null)
        {
            return;
        }

        if (addedItemNames.Contains(item.itemName))
        {
            return;
        }

        acquiredItems.Add(item);
        addedItemNames.Add(item.itemName);
    }

    public List<AcquiredItemData> GetAcquiredItems()
    {
        return new List<AcquiredItemData>(acquiredItems);
    }

    public void ResetAllData()
    {
        totalGold = 0;
        acquiredItems.Clear();
        addedItemNames.Clear();

        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.ResetTimer();
        }

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