using UnityEngine;

public class BossDash : IBossState
{
    public void UpdateState(BossStateMachine controller)
    {
        
    }

    public void OnEnter(BossStateMachine controller)
    {
        controller.transform.position = Vector3.MoveTowards(controller.transform.position, controller.bossController.enemyManager.player.transform.position, controller.bossController.boss.abilitySpeed * Time.deltaTime);
        controller.ChangeState(controller.idleState);
    }

    public void OnExit(BossStateMachine controller)
    {
        
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
