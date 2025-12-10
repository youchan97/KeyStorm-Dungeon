public class MonsterIdleState : CharacterIdleState<Monster>
{
    public MonsterIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        if (character.IsMove)
        {
            stateManager.ChangeState(character.MoveState);
            return;
        }
    }
}
