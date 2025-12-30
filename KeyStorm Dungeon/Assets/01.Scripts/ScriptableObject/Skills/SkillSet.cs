using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SkillSet", menuName = ("ScriptableObject/Skill/SkillSet"))]
public class SkillSet : ScriptableObject
{
    public List<SkillData> skills;
}
