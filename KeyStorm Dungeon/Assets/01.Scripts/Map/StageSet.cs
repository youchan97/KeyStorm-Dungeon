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
    public Room startRoom;
    public List<Room> normalRooms;
    public List<Room> bossRooms;
    public List<Room> treasureRooms;
    public List<Room> shopRooms;
    public TileBase horizontalCorridor;
    public TileBase verticalCorridor;
}
