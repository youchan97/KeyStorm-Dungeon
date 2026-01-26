using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LightningPassiveSKill", menuName = ("ScriptableObject/Skill/LightningPassive"))]
public class LightningPassiveData : PassiveSkillData
{
    public LayerMask enemyLayer;
    public float damage;
    public float boxSize;
    public float hitInterval;
    public int hitCount;
    public EffectData effect;
    public float effectSize;
    public override IPassiveSkill CreatePassiveSkill(PlayerSkill playerSkill)
    {
        return new LightningPassiveSkill(playerSkill, this);
    }
}
