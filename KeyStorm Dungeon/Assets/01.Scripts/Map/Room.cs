using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum RoomType
{
    Start,
    Normal,
    Boss,
    Treasure,
    Shop
}

public class RoomNode
{
    public Vector2Int gridPos;
    public RoomType type;
}

public class Room : MonoBehaviour
{
    public RoomType roomType;

    public Transform doorUp;
    public Transform doorDown;
    public Transform doorLeft;
    public Transform doorRight;

    public Transform GetDoor(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return doorUp;
        if (dir == Vector2Int.down) return doorDown;
        if (dir == Vector2Int.left) return doorLeft;
        if (dir == Vector2Int.right) return doorRight;
        return null;
    }
}
