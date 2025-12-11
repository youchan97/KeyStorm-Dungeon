using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BossMonsterAttackState : MonsterAttackState
{
    private BossMonster boss;
    private Transform playerTransform;
    private Vector3 divePosition;
    private GameObject currentShadow;


    public BossMonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.boss = character as BossMonster;
    }

    public override void EnterState()
    {
        playerTransform = boss.playerTransform;

        if(boss.MonsterRb != null)
        {
            boss.MonsterRb.velocity = Vector2.zero;
        }

        if(playerTransform == null)
        {
            stateManager.ChangeState(boss.CreateIdleState());
            return;
        }

        divePosition = playerTransform.position;

        boss.StartCoroutine(DiveAttackCoroutine());
    }

    private IEnumerator DiveAttackCoroutine()
    {
        Vector3 initialBossPos = boss.transform.position;
        Vector3 peakJumpPos = initialBossPos + Vector3.up * boss.JumpHeight;

        boss.Animator.SetTrigger("IsJump");

        float timer = 0f;
        while (timer < boss.JumpDuration)
        {
            timer += Time.deltaTime;
            boss.transform.position = Vector3.Lerp(initialBossPos, peakJumpPos, timer / boss.JumpDuration);
            yield return null;
        }

        boss.transform.position = peakJumpPos;
        boss.GetComponent<Collider2D>().enabled = false;

        if (boss.ShadowPrefab != null)
        {
            currentShadow = GameObject.Instantiate(boss.ShadowPrefab, divePosition, Quaternion.identity);
            currentShadow.transform.localScale = Vector3.one * boss.MinShadowScale;

            float shadowTimer = 0f;
            while(shadowTimer < boss.ShadowScaleTime)
            {
                shadowTimer += Time.deltaTime;
                float currentScale = Mathf.Lerp(boss.MinShadowScale, boss.MaxShadowScale, shadowTimer / boss.ShadowScaleTime);
                currentShadow.transform.localScale = Vector3.one * currentScale;
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("BossMonsterAttackState: ShadowPrefab이 할당되지 않음.");
        }

        yield return new WaitForSeconds(boss.DiveDelay - boss.ShadowScaleTime);

        if (currentShadow != null)
        {
            Object.Destroy(currentShadow);
        }

        boss.transform.position = divePosition + Vector3.up * boss.JumpHeight * 0.5f;
        boss.GetComponent<Collider2D>().enabled = true;

        //boss.Animator.SetTrigger("IsDive");

        Vector3 startDivePos = boss.transform.position;
        Vector3 endDivePos = divePosition;

        timer = 0f;
        while(timer < boss.DiveDuration)
        {
            timer += Time.deltaTime;
            boss.transform.position = Vector3.Lerp(startDivePos, endDivePos, timer / boss.DiveDuration);
            yield return null;
        }

        boss.transform.position = endDivePos;

        ApplyLandingDamage(endDivePos, boss.MaxShadowScale);

        boss.ResetAttackCooldown();
        stateManager.ChangeState(boss.CreateMoveState());
    }

    private void ApplyLandingDamage(Vector3 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            Player hitPlayer = hitCollider.GetComponent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(boss.Damage);
            }
        }
    }

    public override void ExitState()
    {
        boss.StopAllCoroutines();

        boss.GetComponent<Collider2D>().enabled.MustBeTrue();

        if(currentShadow != null)
        {
            Object.Destroy(currentShadow);
            currentShadow = null;
        }

        boss.Animator.ResetTrigger("IsJump");
        //boss.Animator.ResetTrigger("IsDive");
    }
}
