using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private bool isBoss = false;

    private float currentHp;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHp = maxHp;
        Debug.Log($"[TutorialEnemy] 생성 - HP: {currentHp}, Boss: {isBoss}");
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log($"[TutorialEnemy] 피해 받음 - 데미지: {damage}, 남은 HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashEffect());
        }
    }

    void Die()
    {
        Debug.Log($"[TutorialEnemy] 사망! Boss: {isBoss}");

        if (TutorialManager.Instance != null)
        {
            if (isBoss)
            {
                TutorialManager.Instance.OnBossKilled();

                GameObject portal = GameObject.Find("Portal");
                if (portal != null)
                {
                    portal.SetActive(true);
                }
            }
            else
            {
                TutorialManager.Instance.OnEnemyKilled();
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[TutorialEnemy] 충돌 감지: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("PlayerBullet"))
        {
            Debug.Log("[TutorialEnemy] 플레이어 총알 맞음!");
            TakeDamage(10f);
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Bomb"))
        {
            Debug.Log("[TutorialEnemy] 폭탄 맞음!");
            TakeDamage(50f);
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            Color original = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = original;
        }
    }
}
