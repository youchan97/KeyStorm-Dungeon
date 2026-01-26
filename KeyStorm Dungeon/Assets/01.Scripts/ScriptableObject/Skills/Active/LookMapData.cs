using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LookMapSkill", menuName = ("ScriptableObject/Skill/LookMap"))]
public class LookMapData : ActiveSkillData
{
    public float duration;

    public override IActiveSKill CreateActiveSkill(PlayerSkill playerSkill)
    {
        return new LookMapSkill(playerSkill, this);
    }
}
