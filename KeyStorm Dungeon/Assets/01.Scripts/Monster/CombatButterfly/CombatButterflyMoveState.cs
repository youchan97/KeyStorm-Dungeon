using UnityEngine;

public class CombatButterflyMoveState : MonsterMoveState
{

    private const string ANIM_PARAM_MOVE_X = "MoveX";
    private const string ANIM_PARAM_MOVE_Y = "MoveY";

    public CombatButterflyMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null) return;

        if (character == null || rb == null) return;

        character.FlipSprite(playerTransform);

        float distanceToPlayer = Vector2.Distance(character.transform.position, playerTransform.position);

        Debug.Log("[CombatButterfly] FixedUpdateState: About to call UpdateAnimation().");
        UpdateAnimation();
        Debug.Log("[CombatButterfly] FixedUpdateState: UpdateAnimation() called.");
        UpdateMovement(distanceToPlayer);
        Debug.Log("[CombatButterfly] FixedUpdateState 호출 종료.");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return base.UseFixedUpdate();
    }

    public void UpdateAnimation()
    {
        Debug.Log("--- [CombatButterfly] UpdateAnimation 호출 시작 ---");
        if (character.Animator == null || playerTransform == null) return;
        Debug.Log("--- [CombatButterfly] UpdateAnimation: 모든 참조 유효성 검증 통과 ---");
        Vector2 monsterPos = character.transform.position;
        Vector2 playerPos = playerTransform.position;

        Vector2 desiredMoveDirection = (playerPos - monsterPos).normalized;

        float moveX = desiredMoveDirection.x;
        float moveY = desiredMoveDirection.y;

        Debug.Log($"UpdateAnimation: MoveX = {moveX}, MoveY = {moveY}");

        character.Animator.SetFloat(ANIM_PARAM_MOVE_X, moveX);
        character.Animator.SetFloat(ANIM_PARAM_MOVE_Y, moveY);
        Debug.Log("--- [CombatButterfly] UpdateAnimation 호출 종료 ---");
    }
}
