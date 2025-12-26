using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ConstValue;

public class Door : MonoBehaviour
{
    [SerializeField] Tilemap wallTileMap;
    [SerializeField] BoxCollider2D col;
    [SerializeField] Room room;
    [SerializeField] SpriteRenderer wallSprite;
    [SerializeField] Animator anim;

    public bool canUse;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canUse) return;

        Player player = collision.GetComponent<Player>();

        if (player == null) return;

        if(!IsMonsterRoom())
        {
            col.enabled = !ColliderEnable();
            return;
        }

        if (room.CanOpenDoor) return;

        if(room.IsPlayerIn == false)
            anim.SetBool(DoorAnim, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!canUse) return;

        Player player = collision.GetComponent<Player>();

        if (player == null) return;

        if (!IsMonsterRoom())
        {
            col.enabled = ColliderEnable();
            return;
        }

        if (room.CanOpenDoor) return;

        anim.SetBool(DoorAnim, false);
    }

    bool ColliderEnable()
    {
        return (room.IsPlayerIn && room.CanOpenDoor) || !room.IsPlayerIn;
    }

    public void OpenDoor()
    {
        col.enabled = false;
    }

    public void CloseDoor()
    {
        col.enabled = true;
    }

    public void UseDoor()
    {
        canUse = true;
        if(wallTileMap == null) return;
        wallTileMap?.gameObject.SetActive(false);

        if(IsMonsterRoom())
            wallSprite.enabled = true;
    }

    bool IsMonsterRoom()
    {
        return (room.roomType == RoomType.Normal || room.roomType == RoomType.Boss);
    }

    public void ClearDoor()
    {
        anim.SetBool(DoorAnim, true);
    }
}
