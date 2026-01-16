using System.Collections;
using UnityEngine;
using static ConstValue;

public class PillBugIdleState : MonsterIdleState
{
    private PillBug pillBug;
    private float currentIdleTime;
    private Coroutine coroutine;

    #region 애니메이션
    private const string ReadyRollAnim = "ReadyRoll";
    #endregion

    public PillBugIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        pillBug = character as PillBug;
    }

    public override void EnterState()
    {
        if (pillBug.MonsterRb != null)
        {
            pillBug.MonsterRb.velocity = Vector2.zero;
        }

        if(pillBug.PlayerGO == null)
        {
            return;
        }

        pillBug.ChangeIsChase(false);
        ChoiceDirection();
        currentIdleTime = pillBug.IdleTime;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (pillBug.PlayerTransform == null || pillBug.PlayerGO == null) return;

        if (pillBug.isKnockBack) return;

        Vector2[] rayDirection = {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

        bool playerDetect = false;

        LayerMask detectLayerMask = pillBug.PlayerLayer | pillBug.ObstacleLayer;

        foreach(Vector2 direction in rayDirection)
        {
            RaycastHit2D hit = Physics2D.Raycast(pillBug.transform.position, direction, pillBug.MonsterData.detectRange, detectLayerMask);

            if (hit.collider != null)
            {
                if (((1 << hit.collider.gameObject.layer) & pillBug.PlayerLayer) != 0)
                {
                    playerDetect = true;
                    pillBug.CurrentMoveDirection = direction;
                    break;
                }
                else if (((1 << hit.collider.gameObject.layer) & pillBug.ObstacleLayer) != 0)
                {
                    continue;
                }
            }
        }

        if (playerDetect)
        {
            coroutine = pillBug.StartCoroutine(ReadyRollAnimation());
            return;
        }

        currentIdleTime -= Time.deltaTime;

        if(currentIdleTime <= 0)
        {
            pillBug.MonsterRb.velocity = Vector2.zero;
            ChoiceDirection();
            currentIdleTime = pillBug.IdleTime;
        }

        pillBug.MonsterRb.velocity = pillBug.CurrentMoveDirection * pillBug.MoveSpeed;

        pillBug.Animator.SetFloat(AxisX, pillBug.CurrentMoveDirection.x);
        pillBug.Animator.SetFloat(AxisY, pillBug.CurrentMoveDirection.y);
    }

    public override void ExitState()
    {
        base.ExitState();
        if (coroutine != null)
        {
            pillBug.StopCoroutine(coroutine);
        }
        pillBug.ChangeIsReadyRollAnimationEndToFalse();
        pillBug.Animator.ResetTrigger(ReadyRollAnim);
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private void ChoiceDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 newMoveDirection;
        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            newMoveDirection = directions[Random.Range(0, directions.Length)];
            attempts++;
        } while (newMoveDirection == pillBug.CurrentMoveDirection && attempts < maxAttempts);

        pillBug.CurrentMoveDirection = newMoveDirection;
    }

    private IEnumerator ReadyRollAnimation()
    {
        pillBug.MonsterRb.velocity = Vector2.zero;

        pillBug.Animator.SetTrigger(ReadyRollAnim);
        yield return new WaitUntil(() => pillBug.IsReadyRollAnimtaionEnd == true);

        stateManager.ChangeState(pillBug.CreateMoveState());
    }
}
