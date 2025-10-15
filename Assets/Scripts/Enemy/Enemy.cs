using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public Sprite sprite;
    public EnemyTypes enemyType;
    public AttackType attackType;
    public EnemyAbility ability;
    public int health;
    public int damage;
    public float speed;
    public float attackRadius;
    public float attackSpeed;
    public float abilityTime;
    public float abilityCooldown;
    public float abilityPower;
    public float blockTime;
    public float blockCooldown;
    public float rangedFleeRadius;
    public int rangedPointsToCheck;
    public Projectile projectile;
    public float singDrawSpeed;
    public bool useAltSlash;
    public bool usesBridges;
    public int healthPotionChance;
}
