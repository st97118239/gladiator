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

    [SerializeField] private string spritePath;

    public void Load(Enemy givenEnemy, EnemyManager givenManager)
    {
        enemyManager = givenManager;
        enemy = givenEnemy;
        health = enemy.health;
        spriteRenderer.sprite = enemy.sprite;
        enemyStateMachine.Load();
    }

    public void Hit(int damage)
    {
        health -= damage;

        // To-Do: Knockback code here
        // We can use rigidbody for knockback maybe

        StartCoroutine(HitEffect());

        if (health > 0) return;
        Debug.Log("Killed enemy.");
        enemyManager.CheckIfEnd(this);
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(hitColorTime);

        spriteRenderer.color = Color.white;
    }
}
