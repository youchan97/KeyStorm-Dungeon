using UnityEngine;

public class MeleeMonster : Monster
{
    private MonsterIdleState _idleState;
    private MonsterMoveState _moveState;
    private MeleeMonsterAttackState _attackState;
    private MonsterDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new MonsterIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new MonsterMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new MeleeMonsterAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new MonsterDieState(this, MonsterStateManager);
        return _dieState;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject currentGameObject = collision.gameObject;

        if (CurrentAttackCooldown <= 0f)
        {
            if (currentGameObject.CompareTag("Player"))
            {
                Player player = currentGameObject.GetComponent<Player>();

                if (player != null)
                {
                    Attack(player);
                    Debug.Log("공격!");
                    ResetAttackCooldown();
                }
            }

        }
    }
}
