using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public PlayerStats stats;

    [Header("자원")]
    public int gold;
    public int bombCount;

    [Header("아이템")]
    public List<ItemData> passiveItems = new List<ItemData>();
    public ItemData activeItem;

    public void AddGold(int amount)
    {
        gold += amount;
        HudUI.Instance.UpdateGold(gold);
    }

    public void AddBomb(int amount)
    {
        bombCount += amount;
        HudUI.Instance.UpdateBomb(bombCount);
    }

    public void AddPassiveItem(ItemData data)
    {
        passiveItems.Add(data);
        //stats.ApplyItemStats(data);

        //if (data.attackChange)
        //{
        //    GetComponent<PlayerAttack>()?.ApplyAttackChange(data);
        //} 플레이어쪽에서 추가되면 바꾸면됨
    }

    public void SetActiveItem(ItemData data)
    {
        activeItem = data;
        HudUI.Instance.SetActiveItem(data);
    }
}
