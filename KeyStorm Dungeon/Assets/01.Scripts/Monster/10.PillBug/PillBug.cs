using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBug : MeleeMonster
{
    [Header("공벌레 전용 수치")]
    [SerializeField] private float idleTime;
    [SerializeField] private float moveSpeedMultiple;
    [SerializeField] private float reactionForce;    // 벽에 부딪힌 이후 반작용 할 힘
    [SerializeField] private float reactionDuration; // 벽에 부딪힌 이후 반작용 시간

    private bool isChase;
    private bool isReaction;
    public float IdleTime => idleTime;
    public LayerMask PlayerLayer => playerLayer;
    public float MoveSpeedMultiple => moveSpeedMultiple;
    public bool IsChase => isChase;
    public bool IsReaction => isReaction;

    public Vector2 CurrentMoveDirection { get; set; } = Vector2.zero;

    private PillBugIdleState _idleState;
    private PillBugMoveState _moveState;
    private PillBugAttackState _attackState;
    private PillBugDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new PillBugIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new PillBugMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new PillBugAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new PillBugDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isReaction) return;

        if (isChase)
        {
            GameObject currentGameObject = collision.gameObject;

            if (((1 << currentGameObject.layer) & obstacleLayer) != 0)
            {
                Vector2 reactionDirection = -CurrentMoveDirection.normalized;

                StartCoroutine(obstacleReaction(reactionDirection));
            }
        }
    }

    private IEnumerator obstacleReaction(Vector2 direction)
    {
        isReaction = true;

        if (MonsterRb != null)
        {
            MonsterRb.velocity = Vector2.zero;
            MonsterRb.AddForce(direction * reactionForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(reactionDuration);

        if (MonsterRb != null)
        {
            MonsterRb.velocity = Vector2.zero;
        }

        MonsterStateManager.ChangeState(CreateIdleState());
        isReaction = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    public void ChangeIsChase(bool chase)
    {
        isChase = chase;
    }
}
