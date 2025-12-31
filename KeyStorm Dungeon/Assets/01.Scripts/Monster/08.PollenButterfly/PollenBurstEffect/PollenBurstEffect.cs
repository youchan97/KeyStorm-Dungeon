using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenBurstEffect : MonoBehaviour
{
    [SerializeField] private CircleCollider2D burstCollider;
    [SerializeField] private float duration;
    [SerializeField] private float damageInterval;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int damage;

    private float currentDamageTimer;
    

    private void Awake()
    {
        if (burstCollider == null)
        {
            burstCollider = GetComponent<CircleCollider2D>();
        }

        if (burstCollider != null)
        {
            burstCollider.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        currentDamageTimer = damageInterval;
        Explosion();
        StartCoroutine(pollenDisappears());
    }

    private void Update()
    {
        currentDamageTimer -= Time.deltaTime;
    }

    private void OnDisable()
    {
        currentDamageTimer = damageInterval;
        StopAllCoroutines();
    }

    private void Explosion()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, burstCollider.radius, playerLayer);

        foreach(var col in players)
        {
            var player = col.GetComponent<Player>();
            if (player  != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    private IEnumerator pollenDisappears()
    {
        yield return new WaitForSeconds(duration);

        ObjectPoolManager.Instance.ReturnObject(gameObject, "PollenBurstEffect");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (currentDamageTimer <= 0)
            {
                Player player = collision.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }

                currentDamageTimer = damageInterval;
            }
        }
    }
}
