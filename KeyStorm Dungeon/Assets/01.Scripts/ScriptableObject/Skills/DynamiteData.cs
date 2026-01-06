using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteData : SkillData
{
    public float radius;
    public float duration;

    public override ISkill CreateSkill(PlayerSkill playerSkill)
    {
        return new DynamiteSkill(playerSkill, this);
    }

}
