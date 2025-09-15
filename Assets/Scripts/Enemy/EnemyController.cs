using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyTypes enemyType;
    public int health;
    public int damage;
    public float speed;
    public Vector3 playerLocation;

    [SerializeField] private string spritePath;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Load(Enemy givenEnemy)
    {
        enemyType = givenEnemy.enemyType;
        health = givenEnemy.health;
        damage = givenEnemy.damage;
        spriteRenderer.sprite = Resources.Load<Sprite>(spritePath + enemyType);
    }
}
