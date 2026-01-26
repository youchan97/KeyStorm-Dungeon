using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScratchPassiveSKill", menuName = ("ScriptableObject/Skill/ScratchPassive"))]
public class ScratchPassiveData : PassiveSkillData
{
    public LayerMask enemyLayer;
    public float damageMultiple;
    public float boxSize;
    public float hitInterval;
    public int hitCount;
    public EffectData effect;
    public float effectSize;
    public override IPassiveSkill CreatePassiveSkill(PlayerSkill playerSkill)
    {
        return new ScratchPassiveSkill(playerSkill, this);
    }
}
