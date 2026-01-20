using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "StageRoomData", menuName = "ScriptableObject/StageRoomData")]
public class StageRoomData : ScriptableObject
{
    public Room startRoom;
    public List<Room> normalRooms;
    public List<Room> bossRooms;
    public List<Room> treasureRooms;
    public List<Room> shopRooms;
    public TileBase horizontalCorridor;
    public TileBase verticalCorridor;
}
