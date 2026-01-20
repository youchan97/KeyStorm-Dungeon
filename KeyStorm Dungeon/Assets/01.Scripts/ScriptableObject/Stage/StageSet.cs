using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum StageDifficulty
{
    Easy,
    Normal,
    Hard
}


[CreateAssetMenu(fileName = "StageSet", menuName = "ScriptableObject/StageSet")]
public class StageSet : ScriptableObject
{
    public StageDifficulty stageDifficulty;
    public List<StageData> stageDatas;
}
