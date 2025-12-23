using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class GoldPickup : MonoBehaviour
{
    public int amount = 1;

    private bool magnetMode = false;
    private Transform target;
    private float speed;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (magnetMode && target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed*Time.deltaTime);
        }
    }

    public void EnableMagnet(Transform target, float speed)
    {
        magnetMode = true;
        this.target = target;
        this.speed = speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        inv.AddGold(amount);
        AudioManager.Instance.PlayEffect(GoldSfx);
        ItemDropManager.Instance.ReturnGoldPickup(this);
    }
}
