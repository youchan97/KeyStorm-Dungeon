using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class DesertSlimeAttackState : SlimeAttackState
{
    private DesertSlime desertSlime;

    private Vector2 slideTargetPosition;

    private bool isReadySlideAnimationFinished;
    private bool isSpinAnimationFinished;

    #region 애니메이션
    private const string ReadySlideAnim = "ReadySlide";
    private const string SlideAnim = "Slide";
    #endregion

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;

    public DesertSlimeAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        desertSlime = character as DesertSlime;
    }

    public override void EnterState()
    {
        desertSlime.StopSlide();
        isReadySlideAnimationFinished = false;
        isSpinAnimationFinished = false;

        desertSlime.OnReadySlideAnimation += HandleReadySlideAnimationFinished;
        desertSlime.OnSpinAnimation += HandleSpinAnimationFinished;

        lastPosition = desertSlime.transform.position;
        timeSinceLastCheck = 0f;

        base.EnterState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if (desertSlime.IsSlide)
        {
            float distanceToPosition = Vector2.Distance(desertSlime.transform.position, slideTargetPosition);

            if (distanceToPosition <= desertSlime.SlideStopDistance)
            {
                desertSlime.StopSlide();
            }
            else
            {
                rb.velocity = currentMoveDirection * desertSlime.SlideSpeed;

                timeSinceLastCheck += Time.fixedDeltaTime;
                if (timeSinceLastCheck >= checkInterval)
                {
                    float distanceMoved = Vector3.Distance(desertSlime.transform.position, lastPosition);

                    if(distanceMoved < stoppedThreshold && rb.velocity.sqrMagnitude > stoppedThreshold * stoppedThreshold)
                    {
                        desertSlime.StopSlide();
                    }
                }
            }
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        desertSlime.OnReadySlideAnimation -= HandleReadySlideAnimationFinished;
        desertSlime.OnSpinAnimation -= HandleSpinAnimationFinished;

        if (desertSlime.IsSlide)
        {
            desertSlime.StopSlide();
        }
    }

    protected override IEnumerator AttackCoroutine()
    {
        SlimePattern selectedPattern = SelectPattern();

        switch (selectedPattern)
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
            case SlimePattern.Slide:
                yield return SlidePattern();
                break;
        }

        stateManager.ChangeState(slime.CreateIdleState());
    }

    protected override SlimePattern SelectPattern()
    {
        List<SlimePattern> patterns = new List<SlimePattern>();

        patterns.Add(SlimePattern.JumpMove);
        patterns.Add(SlimePattern.Slam);
        patterns.Add(SlimePattern.Dive);
        patterns.Add(SlimePattern.Slide);

        int randomIndex = Random.Range(0, patterns.Count);
        return patterns[randomIndex];
    }

    private IEnumerator SlidePattern()
    {
        Vector2 direction = (desertSlime.PlayerTransform.position - desertSlime.transform.position).normalized;

        animator.SetFloat(AxisX, direction.x);
        animator.SetTrigger(ReadySlideAnim);

        yield return new WaitUntil(() => isReadySlideAnimationFinished == true);

        slideTargetPosition = slime.PlayerTransform.position;
        currentMoveDirection = (slideTargetPosition - (Vector2)desertSlime.transform.position).normalized;

        animator.SetBool(SlideAnim, true);
        desertSlime.StartSlide();

        yield return new WaitUntil(() => !desertSlime.IsSlide);

        animator.SetBool(SlideAnim, false);
        yield return new WaitUntil(() => isSpinAnimationFinished == true);
    }

    private void HandleReadySlideAnimationFinished()
    {
        isReadySlideAnimationFinished = true;
    }

    private void HandleSpinAnimationFinished()
    {
        isSpinAnimationFinished = true;
    }
}
