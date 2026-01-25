using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BombSKill", menuName = ("ScriptableObject/Skill/Bomb"))]
public class BombData : ActiveSkillData
{
    public override IActiveSKill CreateActiveSkill(PlayerSkill playerSkill)
    {
        return new BombSkill(playerSkill, this);
    }
}
