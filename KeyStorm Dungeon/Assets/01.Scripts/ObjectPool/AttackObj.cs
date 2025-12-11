using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObj : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool isPlayer;

    AttackPoolManager poolManager;
    int damage;
    Vector2 dir;
    float shootSpeed;
    float coolTime;

    Coroutine coroutine;

    public void InitData(Sprite sprite, int value, Vector2 vec, float speed, float cool, AttackPoolManager manager, bool isPlayerAttack)
    {
        spriteRenderer.sprite = sprite;
        damage = value;
        dir = vec;
        shootSpeed = speed;
        coolTime = cool;
        poolManager = manager;

        isPlayer = isPlayerAttack;
        transform.up = dir;

        StartDurate();
    }

    void StartDurate()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(DurateAttack());
    }

    IEnumerator DurateAttack()
    {
        yield return new WaitForSeconds(coolTime);
        poolManager.ReturnPool(this);
    }


    private void FixedUpdate()
    {
        rb.velocity = dir * shootSpeed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Collision"))
        {
            poolManager.ReturnPool(this);
            return;
        }

        Character character;
        if (isPlayer)
            character = collision.GetComponent<Monster>();
        else
            character = collision.GetComponent<Player>();

        if (character == null) return;

        character.TakeDamage(damage);
        poolManager.ReturnPool(this);
        
    }
}
