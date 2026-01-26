using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeteoSKill", menuName = ("ScriptableObject/Skill/Meteo"))]
public class MeteoData : ActiveSkillData
{
    public Meteo meteo;
    public float boxSize;
    public LayerMask enemyLayer;
    public float radius;
    public float duration;
    public float damageMultiple;
    public EffectData explodeEffect;

    [Header("카메라 연출")]
    public float shakePower;
    public float shakeDuration;

    public override IActiveSKill CreateActiveSkill(PlayerSkill playerSkill)
    {
        return new MeteoSkill(playerSkill, this);
    }
}
