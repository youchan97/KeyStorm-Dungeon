using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int amount = 1;

    private bool magnetMode = false;
    private Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (magnetMode && target)
        {
            float speed = 10f;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed*Time.deltaTime);
        }
    }

    public void EnableMagnet()
    {
        magnetMode = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        inv.AddGold(amount);
        Destroy(gameObject);
    }
}
