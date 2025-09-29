using UnityEngine;

public class NymphIdle : IBossState
{
    private Player player;

    public void UpdateState(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float distance = Vector3.Distance(controller.gameObject.transform.position, player.transform.position);

        if (distance < controller.bossController.boss.rangedFleeRadius)
        {
            controller.ChangeState(controller.nymphWalkState);
            return;
        }

        //if (distance <= controller.bossController.boss.attackRadius)
        //{
        //    controller.ChangeState(controller.nymphAttackState);
        //    return;
        //}
    }
    
    public void OnEnter(BossStateMachine controller)
    {
        controller.transform.position = Vector3.zero;
        player = controller.bossController.enemyManager.player;
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {

    }
}
