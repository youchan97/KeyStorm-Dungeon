using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxinCloud : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CircleCollider2D damageCollider;
    [SerializeField] private float damageTickInterval = 2.0f;

    private float toxinDamage;

    private float currentDamageTimer;
    private bool canDamage;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if(damageCollider == null)
        {
            damageCollider = GetComponent<CircleCollider2D>();
        }

        if(damageCollider != null)
        {
            damageCollider.isTrigger = true;
        }
    }

    public void StartToxinCloudEffect(float damage)
    {
        if (animator == null) return;

        toxinDamage = damage;
        gameObject.SetActive(true);

        animator.SetTrigger("IsEffect");

        if (damageCollider != null)
        {
            damageCollider.enabled = true;
        }

        currentDamageTimer = damageTickInterval;
        canDamage = true;
    }

    public void EndToxinCloudEffect()
    {
        canDamage = false;

        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
        gameObject.SetActive (false);
    }

    private void Update()
    {
        if (!canDamage) return;

        currentDamageTimer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canDamage) return;

        if (collision.CompareTag("Player"))
        {
            if(currentDamageTimer <= 0)
            {
                Player player = collision.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(toxinDamage);
                }

                currentDamageTimer = damageTickInterval;
            }
        }
    }
}
