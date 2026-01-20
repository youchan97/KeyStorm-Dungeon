using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerLimitData", menuName = "ScriptableObject/PlayerLimitData")]
public class PlayerLimitData : ScriptableObject
{
    public float maxHp;
    public float minMoveSpeed;
    public float maxMoveSpeed;
    public float minDamage;
    public float minAttackSpeed;
    public float minRange;
    public float minShotSpeed;
    public float minScale;
}
