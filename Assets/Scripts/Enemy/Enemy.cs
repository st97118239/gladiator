using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public EnemyTypes enemyType;
    public int health;
    public int damage;
}
