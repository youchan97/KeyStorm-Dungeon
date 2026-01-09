using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ConstValue;

public enum SlimePattern
{
    JumpMove,
    Slam,
    Dive
}

public class SlimeAttackState : MonsterAttackState
{
    private Slime slime;
    private Coroutine attackCoroutine;
    private GameObject currentShadowInstance;

    private bool isJumpMoveReadyAnimationFinished;
    private bool isJumpMoveLandAnimationFinished;
    private bool isJumpAnimationFinished;

    private Vector2 currentMoveDirection;

    #region 애니메이션
    private const string JumpMoveAnim = "JumpMove";
    private const string SlamAnim = "Slam";
    private const string JumpAnim = "Jump";
    private const string DiveAnim = "Dive";
    private const string AirTimeAnim = "AirTime";
    #endregion

    public SlimeAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        slime = character as Slime;
    }

    public override void EnterState()
    {
        base.EnterState();
        slime.StopMove();

        isJumpMoveReadyAnimationFinished = false;
        isJumpMoveLandAnimationFinished = false;
        isJumpAnimationFinished = false;

        slime.OnJumpMoveReadyAnimation += HandleJumpMoveReadyAnimationFinished;
        slime.OnJumpMoveLandAnimation += HandleJumpMoveLandAnimationFinished;
        slime.OnJumpAnimation += HandleJumpAnimationFinished;

        attackCoroutine = slime.StartCoroutine(AttackCoroutine());
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (slime.PlayerGO == null)
        {
            slime.ChangeStateToPlayerDied();
            return;
        }

        if (slime.IsMove)
        {
            rb.velocity = currentMoveDirection * slime.MoveSpeed;
        }
    }

    public override void ExitState()
    {
        if (attackCoroutine != null)
        {
            slime.StopCoroutine(attackCoroutine);
        }

        slime.OnJumpMoveReadyAnimation -= HandleJumpMoveReadyAnimationFinished;
        slime.OnJumpMoveLandAnimation -= HandleJumpMoveLandAnimationFinished;
        slime.OnJumpAnimation -= HandleJumpAnimationFinished;

        if (slime.IsMove)
        {
            slime.StopMove();
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private IEnumerator AttackCoroutine()
    {
        SlimePattern selectedPattern = SelectPattern();

        switch(selectedPattern)
        {
            case SlimePattern.JumpMove:
                yield return JumpMovePattern();
                break;
            case SlimePattern.Slam:
                yield return SlamPattern();
                break;
            case SlimePattern.Dive:
                yield return DivePattern();
                break;
        }

        stateManager.ChangeState(slime.CreateIdleState());
    }

    private SlimePattern SelectPattern()
    {
        List<SlimePattern> patterns = new List<SlimePattern>();

        patterns.Add(SlimePattern.JumpMove);
        patterns.Add(SlimePattern.Slam);
        patterns.Add(SlimePattern.Dive);

        int randomIndex = Random.Range(0, patterns.Count);
        return patterns[randomIndex];
    }

    private IEnumerator JumpMovePattern()
    {
        for(int i = 0; i < slime.MoveNumber; i++)
        {
            animator.SetTrigger(JumpMoveAnim);
            yield return new WaitUntil(() => isJumpMoveReadyAnimationFinished == true);

            Vector3 offsetMoveDirection = (slime.PlayerTransform.position - slime.transform.position);

            currentMoveDirection = offsetMoveDirection.normalized;

            slime.StartMove();

            yield return new WaitUntil(() => isJumpMoveLandAnimationFinished == true);

            slime.StopMove();

            isJumpMoveReadyAnimationFinished = false;
            isJumpMoveLandAnimationFinished = false;

            yield return new WaitForSeconds(slime.MoveDelay);
        }
    }

    private IEnumerator SlamPattern()
    {
        Vector2 direction = (slime.PlayerTransform.position - slime.transform.position).normalized;

        animator.SetFloat(AxisX, direction.x);
        animator.SetTrigger(SlamAnim);

        yield return new WaitForSeconds(slime.SlamDelay);
    }

    private IEnumerator DivePattern()
    {
        animator.SetTrigger(JumpAnim);

        yield return new WaitUntil(() => isJumpAnimationFinished == true);

        Vector3 initialSlimePosition = slime.transform.position;
        Vector3 groundShadowPosition = initialSlimePosition + Vector3.down * slime.ShadowOffset;
        Vector3 peakJumpPosition = initialSlimePosition + Vector3.up * slime.JumpHeight;

        float timer = 0f;

        slime.GetComponent<Collider2D>().enabled = false;

        if (slime.BossShadowPrefab != null)
        {
            currentShadowInstance = GameObject.Instantiate(slime.BossShadowPrefab, groundShadowPosition, Quaternion.identity);
            currentShadowInstance.transform.localScale = Vector3.one * slime.MaxShadowScale;
        }

        animator.SetTrigger(AirTimeAnim);

        while(timer < slime.JumpDuration)
        {
            timer += Time.deltaTime;
            slime.transform.position = Vector3.Lerp(initialSlimePosition, peakJumpPosition, timer / slime.JumpDuration);

            if (currentShadowInstance != null)
            {
                float currentScale = Mathf.Lerp(slime.MaxShadowScale, slime.MinShadowScale, timer / slime.JumpDuration);
                currentShadowInstance.transform.localScale = Vector3.one * currentScale;
            }

            yield return null;
        }

        slime.transform.position = peakJumpPosition;

        if (currentShadowInstance != null)
        {
            currentShadowInstance.SetActive(false);
        }

        yield return new WaitForSeconds(slime.DiveDelay);

        Vector3 diveTargetPosition = slime.PlayerTransform.position;

        slime.transform.position = diveTargetPosition + Vector3.up * (slime.JumpHeight);

        animator.SetBool(DiveAnim, true);

        Vector3 startDivePosition = slime.transform.position;
        Vector3 endDivePosition = diveTargetPosition;

        timer = 0f;

        currentShadowInstance.transform.position = diveTargetPosition + Vector3.down * slime.ShadowOffset;
        currentShadowInstance.transform.localScale = Vector3.one * slime.MinShadowScale;

        currentShadowInstance.SetActive(true);

        while(timer < slime.ShadowScaleTime)
        {
            timer += Time.deltaTime;

            float currentScale = Mathf.Lerp(slime.MinShadowScale, slime.MaxShadowScale, timer / slime.ShadowScaleTime);
            currentShadowInstance.transform.localScale = Vector3.one * currentScale;

            yield return null;
        }

        yield return new WaitForSeconds(slime.MaxShadowDuration);

        timer = 0f;

        while(timer < slime.DiveDuration)
        {
            timer += Time.deltaTime;
            slime.transform.position = Vector3.Lerp(startDivePosition, endDivePosition, timer / slime.DiveDuration);
            yield return null;
        }

        GameObject.Destroy(currentShadowInstance);

        animator.SetBool(DiveAnim, false);

        slime.transform.position = endDivePosition;

        slime.ApplyLandingDamage(diveTargetPosition, slime.DiveAttackRange);

        yield return null;

        slime.GetComponent<Collider2D>().enabled = true;

        yield return new WaitForSeconds(slime.DiveAttackDelay);
    }

    private void HandleJumpMoveReadyAnimationFinished()
    {
        isJumpMoveReadyAnimationFinished = true;
    }

    private void HandleJumpMoveLandAnimationFinished()
    {
        isJumpMoveLandAnimationFinished = true;
    }

    private void HandleJumpAnimationFinished()
    {
        isJumpAnimationFinished = true;
    }
}
