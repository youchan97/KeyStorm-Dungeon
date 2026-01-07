using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashSKill", menuName = ("ScriptableObject/Skill/Dash"))]
public class DashData : SkillData
{
    public float dashSpeed;
    public float duration;

    public override ISkill CreateSkill(PlayerSkill playerSkill)
    {
        return new DashSkill(playerSkill, this);
    }
}
