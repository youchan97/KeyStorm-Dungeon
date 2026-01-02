using UnityEngine;

public class JellyBugMitosisIdleState : MonsterIdleState
{
    private JellyBugMitosis jellyBugMitosis;

    public JellyBugMitosisIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        jellyBugMitosis = character as JellyBugMitosis;
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

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 newMoveDirection;
        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            newMoveDirection = directions[Random.Range(0, directions.Length)];
            attempts++;
        } while (newMoveDirection == jellyBugMitosis.CurrentMoveDirection && attempts < maxAttempts); ;

        jellyBugMitosis.CurrentMoveDirection = newMoveDirection;

        jellyBugMitosis.FlipSprite(jellyBugMitosis.CurrentMoveDirection.x);

        stateManager.ChangeState(jellyBugMitosis.CreateMoveState());
    }

    public override void FixedUpdateState()
    {
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
