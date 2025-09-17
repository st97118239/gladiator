using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager { get; private set; }
    public Enemy enemy;
    public int health;

    [SerializeField] private string spritePath;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private EnemyStateMachine enemyStateMachine;

    public void Load(Enemy givenEnemy, EnemyManager givenManager)
    {
        enemyManager = givenManager;
        enemy = givenEnemy;
        health = enemy.health;
        spriteRenderer.sprite = Resources.Load<Sprite>(spritePath + enemy.enemyType);
        enemyStateMachine.Load();
    }

    public void Hit(int damage)
    {
        health -= damage;

        if (health > 0) return;
        Debug.Log("Killed enemy.");
        enemyManager.CheckIfEnd(this);
    }
}
