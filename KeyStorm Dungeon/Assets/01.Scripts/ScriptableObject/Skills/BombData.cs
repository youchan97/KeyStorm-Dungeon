using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombData : SkillData
{
    public override ISkill CreateSkill(PlayerSkill playerSkill)
    {
        return new BombSkill(playerSkill, this);
    }
}
