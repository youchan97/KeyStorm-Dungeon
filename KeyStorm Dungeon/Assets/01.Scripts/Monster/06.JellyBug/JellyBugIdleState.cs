using UnityEngine;

public class JellyBugIdleState : MonsterIdleState
{
    private JellyBug jellyBug;

    public JellyBugIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        jellyBug = character as JellyBug;
    }

    public override void EnterState()
    {
        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }

        if (character.PlayerGO == null)
        {
            return;
        }

        Vector2[] directions = {Vector2.up,  Vector2.down, Vector2.left, Vector2.right};
        jellyBug.CurrentMoveDirection = directions[Random.Range(0, directions.Length)];

        jellyBug.FlipSprite(jellyBug.CurrentMoveDirection.x);

        stateManager.ChangeState(jellyBug.CreateMoveState());
    }

    public override void FixedUpdateState()
    {
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
