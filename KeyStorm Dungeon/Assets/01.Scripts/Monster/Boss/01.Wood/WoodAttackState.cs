using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public enum WoodPattern
{
    Dash,
    Dive,
    Root
}

public class WoodAttackState : MonsterAttackState
{
    private Wood wood;
    private Coroutine attackCoroutine;
    private GameObject currentShadowInstance;

    private bool isGetReadyAnimationFinished;
    private bool isJumpAnimationFinished;
    private bool isTakeRootAnimationFinished;

    private Vector2 currentDashDirection;
    #region 애니메이션
    private const string GetReadyAnim = "GetReady";
    private const string DashAnim = "Dash";
    private const string JumpAnim = "Jump";
    private const string DiveAnim = "Dive";
    private const string TakeRootAnim = "TakeRoot";
    #endregion

    private bool isDash;
    public WoodAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        wood = character as Wood;
    }

    public override void EnterState()
    {
        base.EnterState();

        isDash = false;
        isGetReadyAnimationFinished = false;
        isJumpAnimationFinished = false;
        isTakeRootAnimationFinished = false;

        wood.OnReadyToDashAnimation += HandleGetReadyAnimationFinished;
        wood.OnJumpAnimation += HandleJumpAnimationFinished;
        wood.OnTakeRootAnimation += HandleTakeRootAnimationFinished;

        attackCoroutine = wood.StartCoroutine(AttackCoroutine());
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        if (isDash)
        {
            Vector2 raycastStartPosition = wood.WoodCollider.bounds.center;

            Debug.DrawRay(raycastStartPosition, currentDashDirection * wood.DetectStopDistance, Color.red, 0f);
            RaycastHit2D hit = Physics2D.Raycast(
                raycastStartPosition,
                currentDashDirection,
                wood.DetectStopDistance,
                wood.HitToDashStopLayer
            );

            if (hit.collider != null)
            {
                StopDash();
                return;
            }

            animator.SetFloat(AxisX, currentDashDirection.x);
            animator.SetFloat(AxisY, currentDashDirection.y);
            rb.velocity = currentDashDirection * wood.DashMoveSpeed;
        }
    }

    public override void ExitState()
    {
        if (attackCoroutine != null)
        {
            wood.StopCoroutine(attackCoroutine);
        }

        wood.OnReadyToDashAnimation -= HandleGetReadyAnimationFinished;
        wood.OnJumpAnimation -= HandleJumpAnimationFinished;
        wood.OnTakeRootAnimation -= HandleTakeRootAnimationFinished;

        if (isDash)
        {
            StopDash();
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private IEnumerator AttackCoroutine()
    {
        WoodPattern selectedPattern = SelectPattern();

        switch(selectedPattern)
        {
            case WoodPattern.Dash:
                yield return DashPattern();
                break;
            case WoodPattern.Dive:
                yield return DivePattern();
                break;
            /*case WoodPattern.Root:
                yield return RootPattern();
                wood.ResetRootPatternCooldown();
                break;*/
        }

        yield return new WaitForSeconds(wood.AttackDelay);

        stateManager.ChangeState(wood.CreateIdleState());
    }

    private WoodPattern SelectPattern()
    {
        List<WoodPattern> patterns = new List<WoodPattern>();

        patterns.Add(WoodPattern.Dash);
        patterns.Add(WoodPattern.Dive);

        /*if(wood.CurrentRootPatternCooldown <= 0f)
        {
            patterns.Add(WoodPattern.Root);
        }*/

        int randomIndex = Random.Range(0, patterns.Count);
        return patterns[randomIndex];
    }

    private IEnumerator DashPattern()
    {
        
        animator.SetTrigger(GetReadyAnim);
        yield return new WaitUntil(() => isGetReadyAnimationFinished == true);

        currentDashDirection = (wood.PlayerTransform.position - wood.transform.position).normalized;

        animator.SetBool(DashAnim, true);

        StartDash();

        yield return null;

        yield return new WaitUntil(() => !isDash);

        isGetReadyAnimationFinished = false;
        animator.SetBool(DashAnim, false);
    }

    private void StartDash()
    {
        isDash = true;
    }

    private void StopDash()
    {
        isDash = false;
        rb.velocity = Vector2.zero;
    }

    private IEnumerator DivePattern()
    {
        animator.SetTrigger(JumpAnim);

        yield return new WaitUntil(() => isJumpAnimationFinished == true);

        Vector3 initialWoodPosition = wood.transform.position;
        Vector3 groundShadowPosition = initialWoodPosition + Vector3.down * wood.ShadowOffset;
        Vector3 peakJumpPosition = initialWoodPosition + Vector3.up * wood.JumpHeight;

        float timer = 0f;

        wood.GetComponent<Collider2D>().enabled = false;

        if(wood.BossShadowPrefab != null)
        {
            currentShadowInstance = GameObject.Instantiate(wood.BossShadowPrefab, groundShadowPosition, Quaternion.identity);
            currentShadowInstance.transform.localScale = Vector3.one * wood.MaxShadowScale;
        }

        while(timer < wood.JumpDuration)
        {
            timer += Time.deltaTime;
            wood.transform.position = Vector3.Lerp(initialWoodPosition, peakJumpPosition, timer / wood.JumpDuration);
            
            if (currentShadowInstance != null)
            {
                float currentScale = Mathf.Lerp(wood.MaxShadowScale, wood.MinShadowScale, timer / wood.JumpDuration);
                currentShadowInstance.transform.localScale = Vector3.one * currentScale;
            }

            yield return null;
        }

        wood.transform.position = peakJumpPosition;

        if (currentShadowInstance != null)
        {
            currentShadowInstance.SetActive(false);
        }

        yield return new WaitForSeconds(wood.DiveDelay);

        Vector3 yOffset = new Vector3(0f, 1f, 0f);
        Vector3 diveTargetPosition = wood.PlayerTransform.position;
        Vector3 diveTargetPositionYOffset = wood.PlayerTransform.position + yOffset;

        wood.transform.position = diveTargetPosition + Vector3.up * (wood.JumpHeight);

        animator.SetBool(DiveAnim, true);

        Vector3 startDivePosition = wood.transform.position;
        Vector3 endDivePosition = diveTargetPositionYOffset;

        timer = 0f;

        currentShadowInstance.transform.position = diveTargetPositionYOffset + Vector3.down * wood.ShadowOffset;
        currentShadowInstance.transform.localScale = Vector3.one * wood.MinShadowScale;

        currentShadowInstance.SetActive(true);

        while(timer < wood.DiveDuration)
        {
            timer += Time.deltaTime;
            wood.transform.position = Vector3.Lerp(startDivePosition, endDivePosition, timer / wood.DiveDuration);
            if(currentShadowInstance != null)
            {
                float currentScale = Mathf.Lerp(wood.MinShadowScale, wood.MaxShadowScale, timer / wood.DiveDuration);
                currentShadowInstance.transform.localScale = Vector3.one * currentScale;
            }
            yield return null;
        }

        GameObject.Destroy(currentShadowInstance);
        wood.transform.position = endDivePosition;

        wood.ApplyLandingDamage(diveTargetPosition, 2);

        yield return null;

        wood.GetComponent<Collider2D>().enabled = true;

        yield return new WaitForSeconds(wood.AttackDelay);

        animator.SetBool(DiveAnim, false);
        stateManager.ChangeState(wood.CreateIdleState());
    }

    private IEnumerator RootPattern()
    {
        animator.SetTrigger(TakeRootAnim);
        yield return new WaitUntil(() => isTakeRootAnimationFinished == true);

        for (int i = 0; i < wood.SpawnRootQuantity; i++)
        {
            
        }
    }

    private void HandleGetReadyAnimationFinished()
    {
        isGetReadyAnimationFinished = true;
    }

    private void HandleJumpAnimationFinished()
    {
        isJumpAnimationFinished = true;
    }

    private void HandleTakeRootAnimationFinished()
    {
        isTakeRootAnimationFinished = true;
    }
}
