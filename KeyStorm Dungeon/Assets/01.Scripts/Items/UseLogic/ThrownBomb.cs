using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    Transform holder;        // 들고 있는 주인(보통 플레이어)
    bool isHeld = false;     // 들고 있는 상태인지
    bool hasExploded = false;
    float timer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 처음에는 손에 들고 나올 거라 물리 끔
        rb.isKinematic = true;
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
        if (isHeld && holder != null)
        {
            Vector3 offset = new Vector3(0, 1f, 0); // 머리 위로 약간 올려서 표시
            transform.position = holder.position + offset;
        }
    }

    // 플레이어가 폭탄을 들기 시작할 때 호출
    public void Hold(Transform _holder, float _fuseTime)
    {
        holder = _holder;
        isHeld = true;
        fuseTime = _fuseTime;
        timer = 0f;          // 들기 시작한 순간부터 타이머 재시작

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    // 들고 있던 폭탄을 던질 때 호출
    public void Throw(Vector2 direction, float power)
    {
        isHeld = false;
        holder = null;

        rb.isKinematic = false;
        rb.AddForce(direction.normalized * power, ForceMode2D.Impulse);
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // 폭발 이펙트
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 범위 내 피격 판정
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                //일단 몬스터파트랑 연동을 해봐야하는거라 주석처리함
                //var enemyHealth = hit.GetComponent<EnemyHealth>();
                //if (enemyHealth != null)
                //{
                //    enemyHealth.TakeDamage(damageToEnemy);
                //}
            }

            if (hit.CompareTag("Player"))
            {
                //일단 플레이어파트랑 연동을 해봐야하는거라 주석처리함
                //var playerHealth = hit.GetComponent<PlayerHealth>();
                //if (playerHealth != null)
                //{
                //    playerHealth.TakeDamage(damageToPlayer);
                //}
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
