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
        Debug.Log($"[Door] {name} CloseDoor 호출!");

        if (col != null)
        {
            col.enabled = true;
            Debug.Log($"[Door] {name} Collider 활성화!");
        }

        if (wallTileMap != null)
        {
            wallTileMap.gameObject.SetActive(true);
            Debug.Log($"[Door] {name} WallTileMap 활성화!");
        }

        if (wallSprite != null)
        {
            wallSprite.enabled = true;
            Debug.Log($"[Door] {name} WallSprite 활성화!");
        }

        if (anim != null)
        {
            anim.SetBool(DoorAnim, false);
        }
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
        Debug.Log($"[Door] {name} ForceOpen 호출!");

        canUse = true;

        if (col != null)
        {
            col.enabled = false;
            Debug.Log($"[Door] {name} Collider 비활성화!");
        }

        if (wallTileMap != null)
        {
            wallTileMap.gameObject.SetActive(false);
            Debug.Log($"[Door] {name} WallTileMap 비활성화!");
        }

        if (wallSprite != null)
        {
            wallSprite.enabled = false;
            Debug.Log($"[Door] {name} WallSprite 비활성화!");
        }

        if (anim != null)
        {
            anim.SetBool(DoorAnim, true);
        }
    }
}
