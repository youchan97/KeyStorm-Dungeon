using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] BoxCollider2D col;
    [SerializeField] Room room;

    public bool canUse;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canUse) return;

        Player player = collision.GetComponent<Player>();

        if (player == null) return;

        col.enabled = !ColliderEnable();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!canUse) return;

        Player player = collision.GetComponent<Player>();

        if (player == null) return;

        col.enabled = ColliderEnable();
    }

    bool ColliderEnable()
    {
        return (room.IsPlayerIn && room.CanOpenDoor) || !room.IsPlayerIn;
    }

    public void UseDoor() => canUse = true;
}
