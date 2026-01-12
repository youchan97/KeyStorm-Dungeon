using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomChangeEvent", menuName = "ScriptableObject/Event/RoomChangeEvent")]
public class RoomChangeEvent : ScriptableObject
{
    public event Action<Room> OnRoomChange;

    public void RoomChange(Room room)
    {
        OnRoomChange?.Invoke(room);
    }
}
