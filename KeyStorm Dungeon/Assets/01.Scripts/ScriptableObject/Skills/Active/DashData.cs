using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashSKill", menuName = ("ScriptableObject/Skill/Dash"))]
public class DashData : ActiveSkillData
{
    public float dashSpeed;
    public float duration;

    public override IActiveSKill CreateActiveSkill(PlayerSkill playerSkill)
    {
        return new DashSkill(playerSkill, this);
    }
}
