using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemPickup : MonoBehaviour
{
    //[Header("아이템 데이터")]
    //public ItemData itemData;

    //[Header("UI 설정")]
    //public Vector3 uiOffset = new Vector3(0f, 1f, 0f);

    //[Header("먹기 설정")]
    //[SerializeField] private KeyCode pickupKey = KeyCode.E;

    //private bool isPlayerNearby = false;
    //private Collider2D playerCollider;
    //private float exitCooldown = 0f;

    //private void Awake()
    //{
    //    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    //    if (rb != null)
    //    {
    //        rb.bodyType = RigidbodyType2D.Kinematic;
    //        rb.simulated = true;
    //        rb.gravityScale = 0f;
    //        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    //    }
    //}

    //private void OnEnable()
    //{
    //    Invoke(nameof(CheckPlayerInRange), 0.1f);
    //}

    //private void CheckPlayerInRange()
    //{
    //    if (itemData == null) return;

    //    CircleCollider2D col = GetComponent<CircleCollider2D>();
    //    if (col == null || !col.enabled) return;

    //    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, col.radius);

    //    foreach (var hit in hits)
    //    {
    //        if (hit.CompareTag("Player"))
    //        {
    //            Debug.Log($"[ItemPickup] OnEnable 시점 플레이어 감지: {name}");
    //            OnTriggerEnter2D(hit);
    //            break;
    //        }
    //    }
    //}


    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    if (itemData == null)
    //    {
    //        Debug.LogWarning($"[ItemPickup] {name}에 itemData가 없습니다.");
    //        return;
    //    }

    //    if (isPlayerNearby)
    //    {
    //        Debug.LogWarning($"[ItemPickup] 중복 Enter 무시: {name}");
    //        return;
    //    }

    //    isPlayerNearby = true;
    //    playerCollider = other;
    //    exitCooldown = 0.2f;

    //    WorldItemUIController.Instance?.OnItemEnter(this);

    //    Debug.Log($"[ItemPickup]  플레이어 범위 진입: {name}");
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    if (exitCooldown > 0f)
    //    {
    //        Debug.LogWarning($"[ItemPickup] 떨림 감지 - Exit 무시: {name}");
    //        return;
    //    }

    //    isPlayerNearby = false;
    //    playerCollider = null;

    //    WorldItemUIController.Instance?.OnItemExit(this);

    //    Debug.Log($"[ItemPickup]  플레이어 범위 이탈: {name}");
    //}
    //private void Update()
    //{
    //    if (exitCooldown > 0f)
    //    {
    //        exitCooldown -= Time.deltaTime;
    //    }

    //    if (isPlayerNearby && Input.GetKeyDown(pickupKey))
    //    {
    //        PickupItem();
    //    }
    //}

    //private void PickupItem()
    //{
    //    Debug.Log($"[ItemPickup] 아이템 획득: {itemData.itemName}");

    //    isPlayerNearby = false;

    //    if (WorldItemUIController.Instance != null)
    //    {
    //        WorldItemUIController.Instance.OnItemExit(this);
    //    }

    //    Destroy(gameObject);
    //}

    //private void OnDisable()
    //{
    //    CancelInvoke(nameof(CheckPlayerInRange));

    //    // 비활성화 시 무조건 UI 정리
    //    isPlayerNearby = false;

    //    if (WorldItemUIController.Instance != null)
    //    {
    //        WorldItemUIController.Instance.OnItemExit(this);
    //        Debug.Log($"[ItemPickup] OnDisable - UI 정리 완료: {name}");
    //    }
    //}

    //public virtual void ConfigureUI(WorldItemUIWidget widget)
    //{
    //    widget.Bind(this);
    //}
}
