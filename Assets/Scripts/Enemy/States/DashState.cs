using UnityEngine;

public class DashState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {

    }
    
    public void OnEnter(EnemyStateMachine controller)
    {
        controller.rb2d.constraints = RigidbodyConstraints2D.None;
    }

    public void OnExit(EnemyStateMachine controller)
    {
        controller.rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void OnHurt(EnemyStateMachine controller)
    {

    }
}
