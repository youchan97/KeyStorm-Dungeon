// 움직이지 않는 몬스터이기에 사용하지 않음
public class SpitterMoveState : MonsterMoveState
{
    public SpitterMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
    }
}
