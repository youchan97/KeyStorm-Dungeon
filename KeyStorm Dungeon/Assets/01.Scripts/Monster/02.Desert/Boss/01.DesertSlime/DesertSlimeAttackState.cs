using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSlimeAttackState : SlimeAttackState
{
    private DesertSlime desertSlime;

    private bool isReadySlideAnim;

    private const string ReadySlideAnim = "ReadySlide";

    public DesertSlimeAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        desertSlime = character as DesertSlime;
    }

    public override void EnterState()
    {
        isReadySlideAnim = false;

        base.EnterState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if (desertSlime.IsSlide)
        {
            rb.velocity = currentMoveDirection * (desertSlime.MoveSpeed * desertSlime.SlideSpeed);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
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
            /*case SlimePattern.Slide:
                yield return SlidePattern();
                break;*/
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

    /*private IEnumerator SlidePattern()
    {
        animator.SetTrigger(ReadySlideAnim);

        yield return new WaitUntil(() => isReadySlideAnim == true);

        currentMoveDirection = (slime.PlayerTransform.position - desertSlime.transform.position).normalized;

        desertSlime.StartSlide();

    }*/
}
