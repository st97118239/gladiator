using UnityEngine;

public class BossMelee : IBossState
{
    public void UpdateState(BossStateMachine controller)
    {
        
    }

    public void OnEnter(BossStateMachine controller)
    {
        controller.bossController.enemyManager.levelManager.uiManager.audioManager.PlayEnemyAttack();
        controller.bossController.enemyManager.player.PlayerHit(controller.bossController.boss.damage, true);
        controller.StartAttackDelay(false);
    }

    public void OnExit(BossStateMachine controller)
    {
        
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
