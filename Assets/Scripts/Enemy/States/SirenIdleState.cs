using System;
using UnityEngine;

public class SirenIdleState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        if (!controller.puddle) controller.FindPuddle();

        controller.ChangeState(controller.sirenSingState);
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
