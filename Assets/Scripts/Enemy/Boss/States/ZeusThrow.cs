using UnityEngine;

public class ZeusThrow : IBossState
{
    private Transform playerTransform;

    public void UpdateState(BossStateMachine controller)
    {
        
    }

    public void OnEnter(BossStateMachine controller)
    {
        playerTransform = controller.bossController.enemyManager.player.transform;

        Vector3 aimDir = (playerTransform.position - controller.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;

        ProjectileObj proj = controller.bossController.enemyManager.GetProjectile();

        if (!proj)
        {
            controller.ChangeState(controller.zeusWalkState);
            return;
        }

        proj.Load(controller.bossController.boss.projectile, playerTransform.position, null, controller.bossController, null, angle, controller.bossCollider);
        controller.bossController.enemyManager.levelManager.uiManager.audioManager.PlayZeusThrow();

        controller.StartAttackDelay(false);
    }

    public void OnExit(BossStateMachine controller)
    {
        
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
