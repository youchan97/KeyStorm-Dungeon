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

    public abstract ISkill CreateSkill(PlayerSkill playerSkill);
}
