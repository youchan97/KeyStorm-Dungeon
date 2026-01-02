using UnityEngine;
using static ConstValue;

public class MonsterAttackState : CharacterAttackState<Monster>
{
    protected Player player;
    protected Rigidbody2D rb;
    protected Animator animator;
    public MonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;
        
        if(character.PlayerGO != null)
        {
            player = character.PlayerGO.GetComponent<Player>();
            character.SetAttackTarget(player);
            if (player == null)
            {
                Debug.LogError("MonsterAttackState: Player GameObject에 Player컴포넌트가 없음");
            }
        }
        else
        {
            Debug.LogError("MonsterAttackState: Monster.playerGO가 할당되지 않음");
        }

        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }
    }

    public override void UpdateState()
    {
        // 임시로 플레이어의 사망을 체크
        if (character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
            return;
        }

        character.FlipSprite(rb.velocity.x);

        float distanceToPlayer = Vector2.Distance(character.transform.position, player.transform.position);

        if (character is RangerMonster)
        {
            if (distanceToPlayer > character.MonsterData.targetDistance)
            {
                stateManager.ChangeState(character.CreateMoveState());
                return;
            }
        }

        if (character.CurrentAttackCooldown <= 0)
        {
            animator.SetTrigger("IsAttack");
            character.Attack(player);
            character.ResetAttackCooldown();
        }

        
    }

    public override void ExitState()
    {
        if (animator != null)
        {
            animator.ResetTrigger("IsAttack");
        }
        character.SetAttackTarget(null);
    }
}
