using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObject/StageData")]
public class StageData : ScriptableObject
{
    public int stageIndex;
    public int totalRoomCount;
    public int bossRoomCount;
    public int treasureRoomCount;
    public int shopRoomCount;
}
