using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager { get; private set; }
    public EnemyStateMachine enemyStateMachine;
    public Enemy enemy;
    public int health;

    public SpriteRenderer spriteRenderer;
    public float hitColorTime;

    public bool isClosestSiren;

    public void Load(Enemy givenEnemy, EnemyManager givenManager)
    {
        enemyManager = givenManager;
        enemy = givenEnemy;
        health = enemy.health;
        spriteRenderer.sprite = enemy.sprite;
        enemyStateMachine.isReloading = false;
        enemyStateMachine.Load();
    }

    public void Hit(int damage)
    {
        if (enemyStateMachine.isDashing) return;

        enemyStateMachine.currentState.OnHurt(enemyStateMachine);

        if (enemyStateMachine.isBlocking || enemyStateMachine.isDashing) return;
        
        health -= damage;

        if (enemyManager.abilityManager.hasLifesteal)
            enemyManager.player.Lifesteal(damage);

        // To-Do: Knockback code here
        // We can use rigidbody for knockback maybe

        if (gameObject.activeSelf)
            StartCoroutine(HitEffect());

        if (health > 0) return;

        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        spriteRenderer.color = Color.white;
        if (enemy.attackType == AttackType.Sing)
        {
            enemyManager.sirens.Remove(this);
            isClosestSiren = false;
        }

        if (enemyStateMachine.platform)
        {
            if (!enemyStateMachine.reachedPlatform) 
                enemyManager.levelManager.availablePlatforms.Add(enemyStateMachine.platform);
            enemyStateMachine.platform = null;
        }

        enemyStateMachine.currentPlatform = null;

        enemyManager.CheckIfEnd(false);
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(hitColorTime);

        switch (enemyStateMachine.isReloading)
        {
            case true:
                spriteRenderer.color = Color.gray4;
                break;
            case false:
                spriteRenderer.color = Color.white;
                break;
        }
    }
}
