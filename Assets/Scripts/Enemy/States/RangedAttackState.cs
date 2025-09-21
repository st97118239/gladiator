using UnityEngine;

public class RangedAttackState : IEnemyState
{
    // To-Do: Should grab first unused projectile in storage that moves towards player, when the player is hit it'll deal damage

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

        ProjectileObj proj = controller.GetProjectile();

        if (!proj)
        {
            controller.ChangeState(controller.rangedWalkState);
            return;
        }

        proj.Load(enemyController.enemy.projectile, player.transform.position, enemyController, angle);

        controller.StartAttackDelay();
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
