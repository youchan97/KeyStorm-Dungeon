using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DynamiteSKill", menuName = ("ScriptableObject/Skill/Dynamite"))]
public class DynamiteData : SkillData
{
    public GameObject dynamite;
    public int damage;
    public float radius;
    public float duration;
    public EffectData effect;
    public override ISkill CreateSkill(PlayerSkill playerSkill)
    {
        return new DynamiteSkill(playerSkill, this);
    }

}
