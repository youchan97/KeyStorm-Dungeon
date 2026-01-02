using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class SporeFlowerMoveState : MonsterMoveState
{
    private SporeFlower sporeFlower;
    private float currentMoveTime;

    private bool isMoving;
    private bool isChangedAttackAnimation;

    private Coroutine changeMoveCoroutine;
    private Coroutine changeAttackCoroutine;

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;

    private Vector2 directionToPlayer;
    private Vector2 moveDirection;
    public SporeFlowerMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        sporeFlower = character as SporeFlower;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;

        animator.SetTrigger("IsChangeMove");

        float clipLength = GetAnimationClipLength("SporeFlower_ChangeMove");
        changeMoveCoroutine = sporeFlower.StartCoroutine(WaitForAnimationToMove(clipLength));

        isMoving = false;
        isChangedAttackAnimation = false;

        playerTransform = character.PlayerTransform;
        currentMoveTime = sporeFlower.MoveTime;
        lastPosition = sporeFlower.transform.position;
        timeSinceLastCheck = 0f;

        directionToPlayer = (playerTransform.position - sporeFlower.transform.position).normalized;

        moveDirection = -directionToPlayer;
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        if (!isMoving || isChangedAttackAnimation)
        {
            return;
        }

        if (playerTransform == null)
        {
            stateManager.ChangeState(sporeFlower.CreateIdleState());
            return;
        }

        if (character.isKnockBack) return;

        currentMoveTime -= Time.fixedDeltaTime;
        if (currentMoveTime <= 0f)
        {
            rb.velocity = Vector2.zero;
            isChangedAttackAnimation = true;
            float changeAttackClipLength = GetAnimationClipLength("SporeFlower_ChangeAttack");
            changeAttackCoroutine = sporeFlower.StartCoroutine(WaitForAnimationToAttack(changeAttackClipLength));
            return;
        }

        rb.velocity = moveDirection * sporeFlower.MoveSpeed;

        timeSinceLastCheck += Time.fixedDeltaTime;
        if (timeSinceLastCheck >= checkInterval)
        {
            float distanceMoved = Vector3.Distance(sporeFlower.transform.position, lastPosition);

            if (distanceMoved < stoppedThreshold && rb.velocity.sqrMagnitude > stoppedThreshold * stoppedThreshold)
            {
                rb.velocity = Vector2.zero;
                isChangedAttackAnimation = true;
                float changeAttackClipLength = GetAnimationClipLength("SporeFlower_ChangeAttack");
                changeAttackCoroutine = sporeFlower.StartCoroutine(WaitForAnimationToAttack(changeAttackClipLength));
                return;
            }
            lastPosition = sporeFlower.transform.position;
            timeSinceLastCheck = 0f;
        }
    }

    public override void ExitState()
    {
        if (changeMoveCoroutine != null)
        {
            sporeFlower.StopCoroutine(changeMoveCoroutine);
            changeMoveCoroutine = null;
        }
        if (changeAttackCoroutine != null)
        {
            sporeFlower.StopCoroutine(changeAttackCoroutine);
            changeAttackCoroutine = null;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool(MoveAnim, false);
            animator.ResetTrigger("IsChangeMove");
            animator.ResetTrigger("IsChangeAttack");
        }

        isMoving = false;
        isChangedAttackAnimation = false;
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private float GetAnimationClipLength(string clipName)
    {
        var clips = sporeFlower.Animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0f;
    }

    private IEnumerator WaitForAnimationToMove(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        isMoving = true;
        animator.SetBool(MoveAnim, true);
        changeMoveCoroutine = null;
    }

    private IEnumerator WaitForAnimationToAttack(float waitTime)
    {
        animator.SetTrigger("IsChangeAttack");

        yield return new WaitForSeconds(waitTime);

        stateManager.ChangeState(sporeFlower.CreateAttackState());
        changeAttackCoroutine = null;
    }
}
