using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TierTheme
{
    public Sprite frameSprite;        
    public GameObject effectPrefab;   
}

[CreateAssetMenu(fileName = "ItemUIThemeSO", menuName = "Data/ItemUITheme")]
public class ItemUIThemeSO : ScriptableObject
{
    public TierTheme tier0;
    public TierTheme tier1;
    public TierTheme tier2;
    public TierTheme tier3;
    public TierTheme tier4;

    public TierTheme Get(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.Tier0 => tier0,
            ItemTier.Tier1 => tier1,
            ItemTier.Tier2 => tier2,
            ItemTier.Tier3 => tier3,
            ItemTier.Tier4 => tier4,
            _ => tier0
        };
    }
}
