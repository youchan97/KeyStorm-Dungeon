using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    public int maxHp = 6;
    public int hp = 6;
    public float moveSpeed = 5f;
    public float damage = 3f;
    public float specialDamageMultiple = 1f;
    public float damageMultiple = 1f;
    public float attackSpeed = 1f;
    public float attackSpeedMultiple = 1f;
    public float range = 1f;
    public float shotSpeed = 10f;
    public int maxAmmo = 10;
    public int useAmmo = 1;
    public float scale = 1f;

    public void ApplyItemStats(ItemData data)
    {
        maxHp += data.maxHp;
        if (data.maxHp > 0)
        {
            hp += data.maxHp;
            hp = Mathf.Min(hp, maxHp);
        }

        moveSpeed += data.moveSpeed;
        damage += data.damage;
        specialDamageMultiple += data.specialDamageMultiple;
        damageMultiple += data.damageMultiple;
        attackSpeed += data.attackSpeed;
        attackSpeedMultiple += data.attackSpeedMultiple;
        range += data.range;
        shotSpeed += data.shotSpeed;
        maxAmmo += data.maxAmmo;
        useAmmo += data.useAmmo;
        scale += data.scale;

        transform.localScale = Vector3.one * scale;
    }
}
