using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DynamiteSKill", menuName = ("ScriptableObject/Skill/Dynamite"))]
public class DynamiteData : ActiveSkillData
{
    public GameObject dynamite;
    public int damage;
    public float radius;
    public float duration;
    public EffectData effect;
    public override IActiveSKill CreateActiveSkill(PlayerSkill playerSkill)
    {
        return new DynamiteSkill(playerSkill, this);
    }

}
