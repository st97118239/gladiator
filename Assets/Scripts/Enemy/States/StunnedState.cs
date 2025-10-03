using UnityEngine;

public class StunnedState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {

    }

    public void OnEnter(EnemyStateMachine controller)
    {
        controller.isStunned = true;
    }

    public void OnExit(EnemyStateMachine controller)
    {
        controller.isStunned = false;
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
