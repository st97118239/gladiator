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
        health -= damage;

        // To-Do: Knockback code here
        // We can use rigidbody for knockback maybe

        StartCoroutine(HitEffect());

        if (health > 0) return;

        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        spriteRenderer.color = Color.white;

        enemyManager.CheckIfEnd();
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(hitColorTime);

        switch (bossStateMachine.isReloading)
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
