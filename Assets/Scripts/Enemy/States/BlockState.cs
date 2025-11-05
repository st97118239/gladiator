using UnityEngine;

public class BlockState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        controller.enemyController.enemyManager.levelManager.uiManager.audioManager.PlayPlayerBlock();
        controller.StartBlockDelay();
    }

    public void OnExit(EnemyStateMachine controller)
    {

    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
