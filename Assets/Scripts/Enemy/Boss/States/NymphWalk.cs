using UnityEngine;

public class NymphWalk : IBossState
{
    private Player player;
    private float speed;
    private int posIdx;
    private Vector3 posToGoTo;
    private bool reachedPos;

    // To-Do: Nymph has to walk to random ranged position, then if player is near, make them walk to another position where player is *not* near

    public void UpdateState(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float playerDistance = Vector3.Distance(controller.gameObject.transform.position, player.transform.position);

        if (playerDistance <= controller.bossController.boss.attackRadius && !controller.isReloading)
        {
            Vector3 aimDir = (controller.bossController.enemyManager.player.transform.position - controller.transform.position).normalized;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            angle -= 90;

            controller.bossController.enemyManager.SummonRoot(controller.bossController, angle);

            controller.StartAttackDelay();
        }

        switch (reachedPos)
        {
            case false when controller.gameObject.transform.position == posToGoTo:
                reachedPos = true;
                break;
            case true when playerDistance <= controller.bossController.boss.rangedFleeRadius:
                OnEnter(controller);
                return;
        }

        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, posToGoTo, speed * Time.deltaTime);

        //if (playerDistance > controller.bossController.boss.attackRadius && !controller.isReloading) return;

        //controller.ChangeState(controller.nymphAttackState);

        
    }

    public void OnEnter(BossStateMachine controller)
    {
        reachedPos = false;
        player = controller.bossController.enemyManager.player;
        speed = controller.bossController.boss.speed;
        posIdx = Random.Range(0, controller.bossController.enemyManager.rangedPositions.Count);
        posToGoTo = controller.bossController.enemyManager.rangedPositions[posIdx].position;
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {

    }
}
