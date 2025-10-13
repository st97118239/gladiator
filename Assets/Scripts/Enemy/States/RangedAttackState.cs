using UnityEngine;

public class RangedAttackState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        Attack(controller.enemyController.enemyManager.player, controller.enemyController, controller);
    }

    private static void Attack(Player player, EnemyController enemyController, EnemyStateMachine controller)
    {
        Vector3 aimDir = (player.transform.position - controller.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;

        ProjectileObj proj = controller.enemyController.enemyManager.GetProjectile();

        if (!proj)
        {
            controller.ChangeState(controller.rangedWalkState);
            return;
        }

        proj.Load(enemyController.enemy.projectile, player.transform.position, enemyController, null, null, angle, controller.enemyCollider);

        enemyController.enemyManager.levelManager.uiManager.audioManager.PlayEnemyShoot();
        controller.StartAttackDelay();
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
