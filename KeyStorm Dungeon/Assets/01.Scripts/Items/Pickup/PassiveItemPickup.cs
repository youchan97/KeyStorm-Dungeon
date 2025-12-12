using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PassiveItemPickup : MonoBehaviour
{
    public ItemData itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;
        if (itemData == null) return;
        if (itemData.isActiveItem) return;

        inv.AddPassiveItem(itemData);
        player.PlayerStatUpdate(itemData);
        ItemPopupUI.Instance.Show(itemData);
        Destroy(gameObject);
    }
}
