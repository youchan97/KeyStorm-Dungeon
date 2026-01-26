using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Bomb,
    KnockBack,
    Dynamite,
    Dash,
    Lightning,
    Meteo
}

public abstract class SkillData : ScriptableObject
{
    public SkillType skillType;

}

public abstract class ActiveSkillData : SkillData
{
    public abstract IActiveSKill CreateActiveSkill(PlayerSkill playerSkill);
}

public abstract class PassiveSkillData : SkillData
{
    public float interval;
    public abstract IPassiveSkill CreatePassiveSkill(PlayerSkill playerSkill);
}
