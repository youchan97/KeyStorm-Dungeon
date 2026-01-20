using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/Player")]
public class PlayerData : ScriptableObject
{
    public CharacterData characterData;
    public float damageMultiple;
    public float specialDamageMultiple;
    public float attackSpeed;
    public float attackSpeedMultiple;
    public float range;
    public float rangeMultiple;
    public float shootSpeed;
    public int maxAmmo;
    public int useAmmo;
    public int ammo;
    public float xScale;
    public float yScale;
}
