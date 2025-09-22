using UnityEngine;

public class NymphIdle : IBossState
{
    private Player player;
    private float speed;

    public void UpdateState(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float distance = Vector3.Distance(controller.gameObject.transform.position, player.transform.position);

        if (distance > controller.bossController.boss.attackRadius) return;

        controller.ChangeState(controller.nymphAttackState);

        //controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, player.transform.position, speed * Time.deltaTime);
    }
    
    public void OnEnter(BossStateMachine controller)
    {
        controller.transform.position = Vector3.zero;
        player = controller.bossController.enemyManager.player;
        speed = controller.bossController.boss.speed;
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {

    }
}
