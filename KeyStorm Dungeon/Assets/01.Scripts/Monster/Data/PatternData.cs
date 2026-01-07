using UnityEngine;

[System.Serializable]
public class PatternData : ScriptableObject
{
    [Header("패턴 정보")]
    public string patternName;
    public float patternCooldown;
}
