using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ItemDropRoom
{
    None = 0,
    Treasure = 1 << 0,
    Boss = 1 << 1,
    Store = 1 << 2,
}

public enum AttackChangeType
{
    None,
    ShotGun,
    Sniper
}

public enum ItemTier
{
    Tier0 = 0,
    Tier1 = 1,
    Tier2 = 2,
    Tier3 = 3,
    Tier4 = 4
}

public enum ActiveCooldownType
{
    None,      //패시브
    PerRoom,   //방 클리어마다 1씩 차는 타입
    PerTime    //초당 1씩 차는 타입
}

[CreateAssetMenu(menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;
    public string itemName;
    public string itemEnglishName;
    [TextArea] public string description;

    [Header("스프라이트")]
    public Sprite worldSprite;  // 바닥에 있을때 이미지
    public Sprite iconSprite;   // UI이미지

    [Header("분류")]
    public bool attackChange;
    public bool isActiveItem;
    public ItemTier tier;
    public ItemDropRoom dropRoom;
    public AttackChangeType attackChangeType;

    [Header("스탯 변화량")]
    public int maxHp;
    public float moveSpeed;
    public float damage;
    public float specialDamageMultiple;
    public float damageMultiple;
    public float attackSpeed;
    public float attackSpeedMultiple;
    public float range;
    public float shotSpeed;
    public int maxAmmo;
    public int useAmmo;
    public float scale;

    [Header("액티브 스킬 전용")]
    public ActiveCooldownType cooldownType;
    public float cooldownMax;
    public SkillType skillType;
}