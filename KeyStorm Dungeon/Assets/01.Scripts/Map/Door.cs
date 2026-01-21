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
            //col.enabled = !ColliderEnable();
            return;
        }

        if (room.CanOpenDoor) return;

        if(room.IsPlayerIn == false)
        {
            anim.SetBool(DoorAnim, true);
            AudioManager.Instance.PlayEffect(DoorSfx);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!canUse) return;

        Player player = collision.GetComponent<Player>();

        if (player == null) return;

        if (!IsMonsterRoom())
        {
            //col.enabled = ColliderEnable();
            return;
        }

        if (room.CanOpenDoor) return;

        anim.SetBool(DoorAnim, false);
        AudioManager.Instance.PlayEffect(DoorSfx);
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

        if (IsMonsterRoom())
            wallSprite.enabled = true;
        else
            col.enabled = false;
    }

    bool IsMonsterRoom()
    {
        return (room.roomType == RoomType.Normal || room.roomType == RoomType.Boss);
    }

    public void ClearDoor()
    {
        if(wallSprite.enabled)
            anim.SetBool(DoorAnim, true);
    }

    public void ForceOpen()
    {
        Debug.Log($"[Door] ForceOpen 호출됨! col: {col != null}, wallTileMap: {wallTileMap != null}, wallSprite: {wallSprite != null}");

        canUse = true;

        if (col != null)
        {
            col.enabled = false;
            Debug.Log("[Door] Collider 비활성화!");
        }

        if (wallTileMap != null)
        {
            wallTileMap.gameObject.SetActive(false);
            Debug.Log("[Door] WallTileMap 비활성화!");
        }

        if (wallSprite != null)
        {
            wallSprite.enabled = false;
            Debug.Log("[Door] WallSprite 비활성화!");
        }
    }
}
