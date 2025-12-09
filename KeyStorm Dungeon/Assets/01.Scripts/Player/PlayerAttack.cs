using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttack
{
    Player player;

    #region 키 입력 관련
    private readonly string[] keyNames =
    { "Q", "W", "E", "R", "A", "S","D", "F" };

    private readonly Vector2[] shotDir =
        {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1),
    new Vector2(-1, 1), new Vector2(0, -1),new Vector2(-1, -1),new Vector2(-1, 0)};

    Dictionary<string, Vector2> keyDic = new Dictionary<string, Vector2>();

    #endregion

    #region Property
    public int Damage { get; private set; }
    public float DamageMultiple { get; private set; }
    public float SpecialDamageMultiple { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackSpeedMultiple { get; private set; }
    public float Range { get; private set; }
    public float RangeMultiple { get; private set; }
    public float ShootSpeed { get; private set; }
    public int MaxAmmo { get; private set; }
    public int UseAmmo { get; private set; }
    public int Ammo { get; private set; }
    #endregion

    public void InitPlayerAttack(Player player, PlayerData data)
    {
        this.player = player;

        Damage = player.Damage;
        DamageMultiple = data.damageMultiple;
        SpecialDamageMultiple = data.specialDamageMultiple;
        AttackSpeed = data.attackSpeed;
        AttackSpeedMultiple = data.attackSpeedMultiple;
        Range = data.range;
        RangeMultiple = data.rangeMultiple;
        ShootSpeed = data.shootSpeed;
        MaxAmmo = data.maxAmmo;
        UseAmmo = data.useAmmo;
        Ammo = data.ammo;

        ShuffleKey();
    }

    #region 플레이어 능력치 변화
    public void InCreaseDamage(int value) => Damage += value;
    public void InCreaseDamageMultiple(float value) => SpecialDamageMultiple += value;
    public void InCreaseSpecialDamageMultiple(float value) => SpecialDamageMultiple += value;
    public void InCreaseAttackSpeed(float value) => AttackSpeed += value;
    public void InCreaseAttackSpeedMultiple(float value) => AttackSpeedMultiple += value;
    public void InCreaseRange(float value) => Range += value;
    public void InCreaseRangeMultiple(float value) => RangeMultiple += value;
    public void InCreaseShootSpeed(float value) => ShootSpeed += value;
    public void InCreaseMaxAmmo(int value) => MaxAmmo += value;
    public void InCreaseUseAmmo(int value) => UseAmmo += value;
    public void InCreaseAmmo(int value) => Ammo += value;
    #endregion

    void ShuffleKey()
    {
        keyDic.Clear();

        Vector2[] randVec = shotDir.OrderBy(x => Random.Range(0f, 1f)).ToArray();

        for(int i = 0; i < keyNames.Length; i++)
        {
            keyDic[keyNames[i]] = randVec[i];
        }
    }

    public void Shoot(string keyName)
    {
        if (!keyDic.ContainsKey(keyName)) return;

        Vector2 dir = keyDic[keyName].normalized;
        Debug.Log(keyName + dir);
        

        Ammo--;
        if(Ammo == 0)
        {
            ShuffleKey();

        }
    }
}
