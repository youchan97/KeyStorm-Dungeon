using DG.Tweening;
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
    public float damageToEnemy = 100;
    public int damageToPlayer = 1;

    [Header("Dotween")]
    public SpriteRenderer bombSr;
    public float minAlphaValue;
    public float maxAlphaValue;
    public float minBlinkValue;
    public float maxBlinkValue;

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
    bool isThrough;

    Coroutine bombCo;
    Tween bombTween;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {      
        StartBlink(bombSr, fuseTime);
    }

    private void OnDisable()
    {
        bombCo = null;
        OnExplode = null;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (!hasExploded && timer >= fuseTime)
        {
            //Explode();
        }

        if (holder != null)
        {
            Vector3 offset = new Vector3(0, 1f, 0); 
            transform.position = holder.position + offset;
        }
    }

    public void StartBlink(SpriteRenderer sr, float totalTime)
    {
        if(bombCo != null)
        {
            StopCoroutine(bombCo);
            bombCo = null;
        }

        bombCo = StartCoroutine(BlinkRoutine(sr, totalTime));
        //bombSr.DOColor(Color.red, totalTime).SetEase(Ease.Linear);
    }

    IEnumerator BlinkRoutine(SpriteRenderer sr, float totalTime)
    {
        float remain = totalTime;
        Material mat = sr.material;
        while (remain > DefaultZero)
        {
            float ratio = remain / totalTime; 

            float blinkInterval = Mathf.Lerp(maxBlinkValue, minBlinkValue, DefaultOne - ratio);
            float halfBlink = blinkInterval * HalfValue;
            float minAlpha = Mathf.Lerp(maxAlphaValue, minAlphaValue, DefaultOne - ratio);

            bombTween?.Kill(sr);

            bombTween = mat.DOColor(Color.red,"_Color", halfBlink).SetTarget(sr);
            yield return new WaitForSeconds(halfBlink);

            if (sr == null)
                yield break;
            bombTween?.Kill(sr);

            bombTween = mat.DOColor(Color.black, "_Color", halfBlink).SetTarget(sr);
            yield return new WaitForSeconds(halfBlink);

            remain -= blinkInterval;
        }
        bombTween?.Kill(sr);    
        Explode();
    }

    public void InitData(float damage, bool canThrough)
    {
        damageToEnemy = damage;
        isThrough = canThrough;
    }

    public void Hold(Transform _holder)
    {
        canTrigger = false;
        holder = _holder;
        timer = 0f;          

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

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
        if (bombCo != null)
        {
            bombTween?.Kill();
            StopCoroutine(bombCo);
        }

        if (hasExploded) return;
        hasExploded = true;

        ShowExplosionRange(transform.position);

        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<Character>();
            if (damageable == null) continue;

            float damage = hit.GetComponent<Player>() ? damageToPlayer : damageToEnemy;

            damageable.TakeDamage(damage);
        }
        AudioManager.Instance.PlayEffect(BombSfx);
        OnExplode?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canTrigger == false) return;

        Monster monster = collision.GetComponent<Monster>();
        bool isWall = ((1 << collision.gameObject.layer) & WallLayer) != 0;
        if (monster != null || (!isThrough && isWall))
        {
            Explode();
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
