public interface IEnemyState
{
    public void UpdateState(EnemyStateMachine controller);

    public void OnEnter(EnemyStateMachine controller);

    public void OnExit(EnemyStateMachine controller);

    public void OnHurt(EnemyStateMachine controller);
}
