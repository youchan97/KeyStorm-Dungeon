using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName ="ScriptableObject/AttackData")]
public class AttackData : ScriptableObject
{
    [Header("투사체")]
    public Sprite sprite;
    public RuntimeAnimatorController animationController;

    [Header("이펙트")]
    public EffectData effect;
}
