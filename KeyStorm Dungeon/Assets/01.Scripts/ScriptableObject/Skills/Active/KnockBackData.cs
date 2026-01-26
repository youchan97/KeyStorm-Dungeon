using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnockBackSKill", menuName =("ScriptableObject/Skill/KnockBack"))]
public class KnockBackData : ActiveSkillData
{
    public float radius;
    public float force;
    public float duration;

    public override IActiveSKill CreateActiveSkill(PlayerSkill playerSkill)
    {
        return new KnockBackSkill(playerSkill, this);
    }
}
