using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemUIController : MonoBehaviour
{
    #region Singleton
    public static WorldItemUIController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        AutoBind();
        if (widget != null) widget.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    #endregion

    [Header("연결")]
    [SerializeField] private Transform player;
    [SerializeField] private WorldItemUIWidget widget;

    [Header("디버그")]
    [SerializeField] private bool showDebugLog = true;

    // 현재 범위 내 아이템 목록
    private List<PassiveItemPickup> nearbyPassiveItems = new List<PassiveItemPickup>();
    private List<ActiveItemPickup> nearbyActiveItems = new List<ActiveItemPickup>();

    // 현재 표시 중인 아이템
    private object currentItem;
    private bool isCurrentPassive;

    private void AutoBind()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (widget == null)
        {
            widget = FindObjectOfType<WorldItemUIWidget>(true);
        }
    }


    public void OnItemEnterPassive(PassiveItemPickup item)
    {
        if (item == null) return;

        if (nearbyPassiveItems.Contains(item))
        {
            if (showDebugLog)
                Debug.LogWarning($"[WorldItemUI] {item.name}이 이미 목록에 있습니다.");
            return;
        }

        nearbyPassiveItems.Add(item);

        if (showDebugLog)
            Debug.Log($"[WorldItemUI] Passive {item.itemData?.itemName} 진입 (총 {nearbyPassiveItems.Count + nearbyActiveItems.Count}개)");

        SelectNearestItem();
    }

    public void OnItemExitPassive(PassiveItemPickup item)
    {
        if (item == null)
        {
            nearbyPassiveItems.RemoveAll(x => x == null);
            if (nearbyPassiveItems.Count + nearbyActiveItems.Count == 0)
            {
                HideUI();
            }
            return;
        }

        bool removed = nearbyPassiveItems.Remove(item);

        if (showDebugLog && removed)
            Debug.Log($"[WorldItemUI]  Passive {item.itemData?.itemName} 이탈 (남은 {nearbyPassiveItems.Count + nearbyActiveItems.Count}개)");

        if (currentItem == item)
        {
            currentItem = null;
            SelectNearestItem();
        }
    }

    public void OnItemEnterActive(ActiveItemPickup item)
    {
        if (item == null) return;

        if (nearbyActiveItems.Contains(item))
        {
            if (showDebugLog)
                Debug.LogWarning($"[WorldItemUI] {item.name}이 이미 목록에 있습니다.");
            return;
        }

        nearbyActiveItems.Add(item);

        if (showDebugLog)
            Debug.Log($"[WorldItemUI] Active {item.itemData?.itemName} 진입 (총 {nearbyPassiveItems.Count + nearbyActiveItems.Count}개)");

        SelectNearestItem();
    }

    public void OnItemExitActive(ActiveItemPickup item)
    {
        if (item == null)
        {
            nearbyActiveItems.RemoveAll(x => x == null);
            if (nearbyPassiveItems.Count + nearbyActiveItems.Count == 0)
            {
                HideUI();
            }
            return;
        }

        bool removed = nearbyActiveItems.Remove(item);

        if (showDebugLog && removed)
            Debug.Log($"[WorldItemUI] Active {item.itemData?.itemName} 이탈 (남은 {nearbyPassiveItems.Count + nearbyActiveItems.Count}개)");

        if (currentItem == item)
        {
            currentItem = null;
            SelectNearestItem();
        }
    }

    private void SelectNearestItem()
    {
        if (player == null || widget == null)
        {
            Debug.LogWarning("[WorldItemUI] Player 또는 Widget이 없습니다.");
            return;
        }

        nearbyPassiveItems.RemoveAll(item => item == null || !item.gameObject.activeInHierarchy);
        nearbyActiveItems.RemoveAll(item => item == null || !item.gameObject.activeInHierarchy);

        if (nearbyPassiveItems.Count == 0 && nearbyActiveItems.Count == 0)
        {
            HideUI();
            return;
        }

        object nearest = null;
        bool isPassive = false;
        float bestDistance = float.MaxValue;
        Vector3 uiOffset = Vector3.zero;

        foreach (var item in nearbyPassiveItems)
        {
            float dist = Vector2.Distance(player.position, item.transform.position);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                nearest = item;
                isPassive = true;
                uiOffset = item.uiOffset;
            }
        }

        foreach (var item in nearbyActiveItems)
        {
            float dist = Vector2.Distance(player.position, item.transform.position);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                nearest = item;
                isPassive = false;
                uiOffset = item.uiOffset;
            }
        }

        if (nearest != currentItem)
        {
            currentItem = nearest;
            isCurrentPassive = isPassive;

            if (currentItem != null)
            {
                ShowUI(nearest, isPassive, uiOffset);
            }
            else
            {
                HideUI();
            }
        }
    }

    private void ShowUI(object item, bool isPassive, Vector3 uiOffset)
    {
        if (widget == null || item == null) return;

        widget.gameObject.SetActive(true);

        if (isPassive)
        {
            PassiveItemPickup passive = item as PassiveItemPickup;
            passive.ConfigureUI(widget);
            widget.SetTrackingTarget(passive.transform, uiOffset);
        }
        else
        {
            ActiveItemPickup active = item as ActiveItemPickup;
            active.ConfigureUI(widget);
            widget.SetTrackingTarget(active.transform, uiOffset);
        }

        if (showDebugLog)
        {
            string typeName = isPassive ? "Passive" : "Active";
            MonoBehaviour mb = item as MonoBehaviour;
            Debug.Log($"[WorldItemUI] UI 표시: {mb.name} ({typeName})");
        }
    }

    private void HideUI()
    {
        if (widget == null) return;

        widget.gameObject.SetActive(false);
        widget.ClearTrackingTarget();

        if (showDebugLog)
            Debug.Log($"[WorldItemUI] UI 숨김");
    }
}