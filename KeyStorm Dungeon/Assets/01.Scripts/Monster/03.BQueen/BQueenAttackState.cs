using System.Collections;
using UnityEngine;
using static ConstValue;

public class BQueenAttackState : MonsterAttackState
{
    private BQueen bQueen;

    private Coroutine attackCoroutine;

    public BQueenAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        bQueen = character as BQueen;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;

        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }

        animator.SetFloat(AxisX, 0f);
        animator.SetFloat(AxisY, 0f);

        animator.SetTrigger("IsAttack");

        float attackAnimationLength = GetAnimationClipLength("BQueen_SpawnBWorker");

        attackCoroutine = bQueen.StartCoroutine(WaitForAttackAnimation(attackAnimationLength));
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        if (attackCoroutine != null)
        {
            bQueen.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        animator.ResetTrigger("IsAttack");
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (bQueen.Animator == null || bQueen.Animator.runtimeAnimatorController == null) return 0f;

        var clips = bQueen.Animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if(clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0f;
    }

    private IEnumerator WaitForAttackAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        stateManager.ChangeState(bQueen.CreateIdleState());
        attackCoroutine = null;
    }
}
