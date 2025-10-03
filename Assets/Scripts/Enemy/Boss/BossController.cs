using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public EnemyManager enemyManager { get; private set; }
    public BossStateMachine bossStateMachine;
    public Boss boss;
    public int health;

    public SpriteRenderer spriteRenderer;
    public float hitColorTime;

    public bool isClosestSiren;

    public void Load(Boss givenBoss, EnemyManager givenManager)
    {
        enemyManager = givenManager;
        boss = givenBoss;
        health = boss.health;
        spriteRenderer.sprite = boss.sprite;
        bossStateMachine.Load();
    }

    public void Hit(int damage)
    {
        if (bossStateMachine.isDashing) return;

        bossStateMachine.currentState.OnHurt(bossStateMachine);

        if (bossStateMachine.isDashing) return;

        health -= damage;

        if (enemyManager.abilityManager.hasLifesteal)
            enemyManager.player.Lifesteal(damage);

        // To-Do: Knockback code here
        // We can use rigidbody for knockback maybe

        StartCoroutine(HitEffect());

        if (health > 0) return;

        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        spriteRenderer.color = enemyManager.defaultEnemyColor; // Sprite Color

        enemyManager.CheckIfEnd(true);
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = enemyManager.hitEnemyColor; // Sprite Color

        yield return new WaitForSeconds(hitColorTime);

        switch (bossStateMachine.isReloading)
        {
            case true:
                spriteRenderer.color = enemyManager.cooldownEnemyColor; // Sprite Color
                break;
            case false:
                spriteRenderer.color = enemyManager.defaultEnemyColor; // Sprite Color
                break;
        }
    }

    public void Stun(float duration)
    {
        bossStateMachine.ChangeState(bossStateMachine.stunnedState);

        StartCoroutine(StunTime(duration));
    }

    private IEnumerator StunTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        bossStateMachine.ChangeState(bossStateMachine.idleState);
    }
}
