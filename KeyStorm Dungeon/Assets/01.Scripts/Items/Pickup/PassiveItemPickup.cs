using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PassiveItemPickup : MonoBehaviour
{
    public ItemData itemData;
    private ItemPickupView view;

    // 상점 진열 여부 (StoreSlot에서 true로 세팅)
    [HideInInspector] public bool isShopDisplay = false;

    private void Awake()
    {
        view = GetComponent<ItemPickupView>();
        if (view != null) view.Apply(itemData);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 상점 진열 아이템은 픽업 금지 (구매는 StoreSlot에서만)
        if (isShopDisplay) return;

        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        if (itemData == null) return;
        if (itemData.isActiveItem) return;

        // 인벤 모델/UI 반영
        FindObjectOfType<InventoryModel>()?.Add(itemData);
        FindObjectOfType<InventoryUIController>()?.Refresh();

        inv.AddPassiveItem(itemData);
        player.UpdatePlayerData(itemData);
        ItemPoolManager.Instance?.MarkAcquired(itemData);

        Destroy(gameObject);
    }
}
