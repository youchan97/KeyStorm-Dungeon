using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightningSKill", menuName = ("ScriptableObject/Skill/Lightning"))]
public class LightningData : ActiveSkillData
{
    public int count;
    public float boxSize;
    public float interval;
    public float size;
    public float damageMultiple;
    public LayerMask enemyLayer;
    public EffectData lightningEffect;
    public override IActiveSKill CreateActiveSkill(PlayerSkill playerSkill)
    {
        return new LightningSkill(playerSkill, this);
    }
}
