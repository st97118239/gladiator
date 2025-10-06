using UnityEngine;

public class GriffonSwipe : IBossState
{
    // TO-DO: Make the boss do a platform swipe attack. Animation not needed rn, is for polishing
    public void UpdateState(BossStateMachine controller)
    {
        
    }

    public void OnEnter(BossStateMachine controller)
    {
        controller.bossController.enemyManager.player.PlayerHit(controller.bossController.boss.extraAttackDamage, true);
        controller.StartAttackDelay(true);
    }

    public void OnExit(BossStateMachine controller)
    {
        
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
