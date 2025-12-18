using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemPickupView : MonoBehaviour
{
    [Header("Refs")]
    public SpriteRenderer sr;

    [Header("Auto")]
    public ItemData itemData;

    private void Reset()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Apply(ItemData data)
    {
        itemData = data;
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        if (data != null && data.worldSprite != null)
        {
            sr.sprite = data.worldSprite;
            sr.color = Color.white;
            sr.enabled = true;
        }
        else
        {
            sr.sprite = null;
            sr.enabled = false;
        }
    }
}
