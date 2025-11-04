using UnityEngine;

public class BlockState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        controller.enemyController.enemyManager.levelManager.uiManager.audioManager.PlayPlayerBlock();
        controller.enemyController.animator.SetBool("Block", true);
        controller.StartBlockDelay();
    }

    public void OnExit(EnemyStateMachine controller)
    {
        controller.enemyController.animator.SetBool("Block", false);
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
