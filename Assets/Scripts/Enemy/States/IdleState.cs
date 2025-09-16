using UnityEngine;

public class IdleState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        if (!controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.walkState);
        }
        else
        {
            Debug.Log("Player is fucking dead;");
        }
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
