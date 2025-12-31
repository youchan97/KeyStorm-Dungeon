using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "ScriptableObject/EffectData")]
public class EffectData : ScriptableObject
{
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
}
