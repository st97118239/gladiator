using UnityEngine;

public class JumpState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        controller.enemyController.enemyManager.levelManager.uiManager.audioManager.PlayStoneGolemJump();
        controller.platform.Break();
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
