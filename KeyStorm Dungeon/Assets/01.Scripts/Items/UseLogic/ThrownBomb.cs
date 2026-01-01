using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class ThrownBomb : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("설정")]
    public float fuseTime = 3f;          // 터지기까지 시간
    public float explosionRadius = 2f;   // 폭발 범위
    public int damageToEnemy = 100;
    public int damageToPlayer = 1;

    [Header("이펙트")]
    public GameObject explosionEffectPrefab;

    [Header("범위 표시")]
    public GameObject explosionRangePrefab;
    public float rangeVisibleTime = 0.3f;

    Transform holder;        // 들고 있는 주인(보통 플레이어)
    bool hasExploded = false;
    float timer = 0f;

    public event Action OnExplode;

    bool canTrigger;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        canTrigger = false;
    }

    private void OnDisable()
    {
        OnExplode = null;
    }

    void Update()
    {
        // 시간 흐름
        timer += Time.deltaTime;
        if (!hasExploded && timer >= fuseTime)
        {
            Explode();
        }

        // 들고 있는 상태면 항상 holder 위에 위치
        if (holder != null)
        {
            Vector3 offset = new Vector3(0, 1f, 0); // 머리 위로 약간 올려서 표시
            transform.position = holder.position + offset;
        }
    }

    // 플레이어가 폭탄을 들기 시작할 때 호출
    public void Hold(Transform _holder)
    {
        holder = _holder;
        timer = 0f;          // 들기 시작한 순간부터 타이머 재시작

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    // 들고 있던 폭탄을 던질 때 호출
    public void Throw(Vector2 direction, float power)
    {
        holder = null;

        canTrigger = true;

        rb.AddForce(direction.normalized * power, ForceMode2D.Impulse);
    }

    void ShowExplosionRange(Vector3 pos)
    {
        if (explosionRangePrefab == null) return;

        GameObject rangeObj = Instantiate(
            explosionRangePrefab,
            pos,
            Quaternion.identity
        );

        float diameter = explosionRadius * 2f;
        rangeObj.transform.localScale = new Vector3(diameter, diameter, 1f);

        Destroy(rangeObj, rangeVisibleTime);
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // 폭탄 범위 표시
        ShowExplosionRange(transform.position);

        // 기존 폭발 이펙트
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // 데미지 판정
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<Character>();
            if (damageable == null) continue;

            int damage = hit.CompareTag("Player") ? damageToPlayer : damageToEnemy;
            damageable.TakeDamage(damage);
        }

        OnExplode?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canTrigger == false) return;

        Monster monster = collision.GetComponent<Monster>();
        bool isWall = ((1 << collision.gameObject.layer) & WallLayer) != 0;
        if (monster != null || isWall)
            Explode();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
