using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ActiveItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public Vector3 uiOffset = new Vector3(0f, 1f, 0f);

    [Header("거리 설정")]
    [SerializeField] private float uiDisplayRadius = 1.5f;
    [SerializeField] private float pickupRadius = 0.5f;

    private ItemPickupView view;
    private Collider2D col;

    [HideInInspector] public bool isShopDisplay = false;

    private bool hasNotifiedUI = false;
    private bool isPickedUp = false;
    private CircleCollider2D uiCollider;
    private CircleCollider2D pickupCollider;

    private void Awake()
    {
        uiDisplayRadius = 1.5f;
        pickupRadius = 0.5f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        col = GetComponent<Collider2D>();
        uiCollider = col as CircleCollider2D;
        if (uiCollider != null)
        {
            uiCollider.isTrigger = true;
            //uiCollider.radius = uiDisplayRadius;
        }

        /*pickupCollider = gameObject.AddComponent<CircleCollider2D>();
        pickupCollider.isTrigger = true;
        pickupCollider.radius = pickupRadius;*/

        view = GetComponent<ItemPickupView>();
        if (view != null)
            view.Apply(itemData);

        if (col != null)
            col.enabled = false;

        StartCoroutine(EnablePickupAfterDelay());
    }

    public void SetData(ItemData data)
    {
        itemData = data;
        if (view != null)
            view.Apply(data);
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        if (col != null)
            col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isPickedUp) return;

        if (!hasNotifiedUI && itemData != null)
        {
            hasNotifiedUI = true;
            WorldItemUIController.Instance?.OnItemEnterActive(this);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isShopDisplay) return;
        if (isPickedUp) return;

        float distance = Vector2.Distance(transform.position, other.transform.position);

        if (distance > pickupRadius) return;

        isPickedUp = true;

        var inv = other.GetComponent<PlayerInventory>();
        var player = other.GetComponent<Player>();

        if (inv == null || player == null) return;
        if (itemData == null || !itemData.isActiveItem) return;

        inv.SetActiveItem(itemData);
        ItemPoolManager.Instance?.MarkAcquired(itemData);

        if (hasNotifiedUI)
        {
            WorldItemUIController.Instance?.OnItemExitActive(this);
            hasNotifiedUI = false;
        }

        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        float distance = Vector2.Distance(transform.position, other.transform.position);

        if (distance > uiDisplayRadius && hasNotifiedUI)
        {
            hasNotifiedUI = false;
            WorldItemUIController.Instance?.OnItemExitActive(this);
        }
    }

    private void OnDisable()
    {
        if (hasNotifiedUI && WorldItemUIController.Instance != null)
        {
            WorldItemUIController.Instance.OnItemExitActive(this);
            hasNotifiedUI = false;
        }
    }

    public void ConfigureUI(WorldItemUIWidget widget)
    {
        widget.Bind(itemData);
    }
}
