using UnityEngine;

public class BossDash : IBossState
{
    public void UpdateState(BossStateMachine controller)
    {
        
    }

    public void OnEnter(BossStateMachine controller)
    {
        controller.bossController.enemyManager.levelManager.uiManager.audioManager.PlayPlayerDash();
        controller.rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void OnExit(BossStateMachine controller)
    {
        controller.rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
