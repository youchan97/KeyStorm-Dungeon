using System;
using System.Collections;
using UnityEngine;

public class BossMonsterAttackState : MonsterAttackState
{
    private BossMonster boss;
    private Transform playerTransform;
    private Vector3 diveTargetPosition;
    private GameObject currentShadowInstance;

    private bool _isJumpAnimationFinished = false;

    public BossMonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.boss = character as BossMonster;
    }

    public override void EnterState()
    {

        playerTransform = boss.PlayerTransform;

        if (boss.MonsterRb != null)
        {
            boss.MonsterRb.velocity = Vector2.zero;
        }

        if (playerTransform == null)
        {
            stateManager.ChangeState(boss.CreateIdleState());
            return;
        }

        _isJumpAnimationFinished = false;
        boss.OnJumpAnimationFinished += HandleJumpAnimationFinished;

        boss.StartCoroutine(DiveAttack());
    }

    public override void UpdateState()
    {
        character.FlipSprite(character.PlayerTransform);
    }

    private IEnumerator DiveAttack()
    {
        boss.Animator.SetTrigger("IsJump");

        yield return new WaitUntil(() => _isJumpAnimationFinished == true);


        Vector3 initialBossPos = boss.transform.position;
        Vector3 groundShadowPos = initialBossPos + Vector3.up * -0.5f;
        Vector3 peakJumpPos = initialBossPos + Vector3.up * boss.JumpHeight;

        float timer = 0f;

        boss.GetComponent<Collider2D>().enabled = false;

        if (boss.BossShadowPrefab != null)
        {
            // 풀링 필요
            currentShadowInstance = GameObject.Instantiate(boss.BossShadowPrefab, groundShadowPos, Quaternion.identity);
            currentShadowInstance.transform.localScale = Vector3.one * boss.MaxShadowScale;
        }
        
        while (timer < boss.JumpDuration)
        {
            timer += Time.deltaTime;
            boss.transform.position = Vector3.Lerp(initialBossPos, peakJumpPos, timer / boss.JumpDuration);
            
            if (currentShadowInstance != null)
            {
                float currentScale = Mathf.Lerp(boss.MaxShadowScale, boss.MinShadowScale, timer / boss.JumpDuration);
                currentShadowInstance.transform.localScale = Vector3.one * currentScale;
            }

            yield return null;
        }

        boss.transform.position = peakJumpPos;

        if (currentShadowInstance != null)
        {
            currentShadowInstance.SetActive(false);
        }

        yield return new WaitForSeconds(boss.DiveDelay);

        diveTargetPosition = playerTransform.position;

        boss.transform.position = diveTargetPosition + Vector3.up * (boss.JumpHeight);

        boss.GetComponent<Collider2D>().enabled = true;

        boss.Animator.SetTrigger("IsDive");

        Vector3 startDivePos = boss.transform.position;
        Vector3 endDivePos = diveTargetPosition;

        timer = 0f;

        currentShadowInstance.transform.position = diveTargetPosition + Vector3.up * -0.5f;
        boss.BossShadowPrefab.transform.localScale = Vector3.one * boss.MinShadowScale;

        currentShadowInstance.SetActive(true);

        while(timer < boss.DiveDuration)
        {
            timer += Time.deltaTime;
            boss.transform.position = Vector3.Lerp(startDivePos, endDivePos, timer / boss.DiveDuration);
            if (currentShadowInstance != null)
            {
                float currentScale = Mathf.Lerp(boss.MinShadowScale, boss.MaxShadowScale, timer / boss.DiveDuration);
                currentShadowInstance.transform.localScale = Vector3.one * currentScale;
            }
            yield return null;
        }

        // 풀링 필요
        GameObject.Destroy(currentShadowInstance);
        boss.transform.position = endDivePos;

        boss.ApplyLandingDamage(diveTargetPosition, 2);

        yield return new WaitForSeconds(boss.LandedDelay);

        boss.ResetPatternCooldown();
        stateManager.ChangeState(boss.CreateMoveState());
    }

    private void HandleJumpAnimationFinished()
    {
        _isJumpAnimationFinished = true;
    }

    public override void ExitState()
    {
        boss.StopAllCoroutines();

        boss.OnJumpAnimationFinished -= HandleJumpAnimationFinished;

        if (boss.BossShadowPrefab != null)
        {
            GameObject.Destroy(currentShadowInstance);
            currentShadowInstance = null;
        }

        boss.GetComponent<Collider2D>().enabled = true;

        boss.Animator.ResetTrigger("IsJump");
        boss.Animator.ResetTrigger("IsDive");
        Debug.Log($"[{boss.name}]: ---- AttackState ExitState 퇴장! (종료) Time: {Time.time}");
    }
}