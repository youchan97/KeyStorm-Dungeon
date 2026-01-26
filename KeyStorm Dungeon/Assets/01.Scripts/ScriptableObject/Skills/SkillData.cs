using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Active,
    Passive
}

public enum ActiveSkillType
{
    Bomb,
    KnockBack,
    Dynamite,
    Dash,
    Lightning,
    Meteo
}

public enum PassiveSkillType
{
    Lightning,
    Scratch
}

public abstract class SkillData : ScriptableObject
{
    public SkillType skillType;

}

public abstract class ActiveSkillData : SkillData
{
    public ActiveSkillType activeSkillType;
    public abstract IActiveSKill CreateActiveSkill(PlayerSkill playerSkill);
}

public abstract class PassiveSkillData : SkillData
{
    public PassiveSkillType passiveSkillType;
    public float interval;
    public abstract IPassiveSkill CreatePassiveSkill(PlayerSkill playerSkill);
}
