using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemUIController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public ItemDescriptionView widgetView;
    public RectTransform widgetRoot;

    [Header("UI Space")]
    public Camera cam;
    public Canvas parentCanvas;

    [Header("Detect (Physics2D OverlapCircle)")]
    public LayerMask itemLayer;
    public float tileRange = 3f;
    public float tileSize = 1f;

    [Header("UI")]
    public Vector3 uiOffset = new Vector3(0f, 1f, 0f);

    private ItemData _lastData;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (parentCanvas == null && widgetRoot != null)
            parentCanvas = widgetRoot.GetComponentInParent<Canvas>();

        if (widgetRoot != null) widgetRoot.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player == null || widgetRoot == null || widgetView == null || cam == null || parentCanvas == null)
            return;

        float radius = tileRange * tileSize;

        var hits = Physics2D.OverlapCircleAll(player.position, radius, itemLayer);

        if (hits == null || hits.Length == 0)
        {
            widgetRoot.gameObject.SetActive(false);
            _lastData = null;
            return;
        }

        Collider2D nearest = null;
        float best = float.MaxValue;

        foreach (var col in hits)
        {
            if (col == null) continue;

            float d = (col.transform.position - player.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                nearest = col;
            }
        }

        if (nearest == null)
        {
            widgetRoot.gameObject.SetActive(false);
            _lastData = null;
            return;
        }

        ItemPickup pickup = nearest.GetComponentInParent<ItemPickup>();
        if (pickup == null || pickup.itemData == null)
        {
            widgetRoot.gameObject.SetActive(false);
            _lastData = null;
            return;
        }

        widgetRoot.gameObject.SetActive(true);

        if (_lastData != pickup.itemData)
        {
            widgetView.SetItem(pickup.itemData);
            _lastData = pickup.itemData;
        }

        Vector3 worldPos = pickup.transform.position + uiOffset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = parentCanvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out Vector2 localPoint);

        widgetRoot.anchoredPosition = localPoint;
        Debug.Log("hits=" + Physics2D.OverlapCircleAll(player.position, tileRange * tileSize, itemLayer).Length);
    }

#if UNITY_EDITOR
    // 에디터에서 탐색 반경 시각화(선택사항)
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        float radius = tileRange * tileSize;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, radius);
    }
#endif
}

