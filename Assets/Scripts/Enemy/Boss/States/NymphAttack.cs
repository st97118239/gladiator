using UnityEngine;

public class NymphAttack : IBossState
{
    private Transform playerTransform;

    public void UpdateState(BossStateMachine controller)
    {
        
    }

    public void OnEnter(BossStateMachine controller)
    {
        //controller.bossController.enemyManager.player.PlayerHit(controller.bossController.boss.damage);

        // Nymph creates a line from itself to +Y of itself
        // That line hits the player on impact

        playerTransform = controller.bossController.enemyManager.player.transform;

        Vector3 aimDir = (playerTransform.position - controller.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;

        Debug.Log(angle); // DEBUGGING

        controller.bossController.enemyManager.SummonRoot(controller.bossController, angle);

        controller.StartAttackDelay();
    }

    public void OnExit(BossStateMachine controller)
    {
        
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
