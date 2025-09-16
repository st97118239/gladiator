using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager { get; private set; }
    public Enemy enemy;
    public int health;

    [SerializeField] private string spritePath;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Load(Enemy givenEnemy, EnemyManager givenManager)
    {
        enemyManager = givenManager;
        enemy = givenEnemy;
        health = enemy.health;
        spriteRenderer.sprite = Resources.Load<Sprite>(spritePath + enemy.enemyType);
    }
}
