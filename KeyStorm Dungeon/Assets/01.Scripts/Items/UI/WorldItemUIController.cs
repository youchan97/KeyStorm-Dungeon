using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;
    [SerializeField] private ItemDescriptionView widget; // World Space 위젯의 ItemDescriptionView
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.2f, 0f);

    [Header("Detect")]
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private int tileRange = 3;
    [SerializeField] private float refreshInterval = 0.1f;

    private ItemPickup _current;
    private float _t;

    private void Awake()
    {
        if (widget != null) widget.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player == null || widget == null) return;

        _t += Time.deltaTime;
        if (_t >= refreshInterval)
        {
            _t = 0f;
            FindNearest();
        }

        if (_current != null)
            widget.transform.position = _current.transform.position + offset;
    }

    private void FindNearest()
    {
        float radius = tileRange * tileSize;
        var hits = Physics2D.OverlapCircleAll(player.position, radius, itemLayer);

        ItemPickup best = null;
        float bestSqr = float.MaxValue;

        foreach (var h in hits)
        {
            var p = h.GetComponent<ItemPickup>();
            if (p == null || p.itemData == null) continue;

            float sqr = (p.transform.position - player.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = p;
            }
        }

        if (best != _current)
        {
            _current = best;
            if (_current == null) widget.SetItem(null);
            else
            {
                widget.SetItem(_current.itemData);
                widget.transform.position = _current.transform.position + offset;
            }
        }
        else
        {
            if (_current != null)
            {
                float r2 = radius * radius;
                if ((_current.transform.position - player.position).sqrMagnitude > r2)
                {
                    _current = null;
                    widget.SetItem(null);
                }
            }
        }
    }
}
