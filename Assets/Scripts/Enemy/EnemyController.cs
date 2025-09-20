using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager { get; private set; }
    public Enemy enemy;
    public int health;

    public SpriteRenderer spriteRenderer;
    public float hitColorTime;

    [SerializeField] private string spritePath;
    [SerializeField] private EnemyStateMachine enemyStateMachine;

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

        // Knockback code hier

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
