using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObject/StageData")]
public class StageData : ScriptableObject
{
    public int stageIndex;
    public StageRoomData roomData;
    public int totalRoomCount;
    public int bossRoomCount;
    public Room bossRoom;
    public int treasureRoomCount;
    public int shopRoomCount;
}
