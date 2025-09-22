using UnityEngine;

[CreateAssetMenu(menuName = "Boss")]
public class Boss : ScriptableObject
{
    public Sprite sprite;
    public EnemyTypes enemyType;
    public AttackType attackType;
    public BossAbility abilityType;
    public int health;
    public int damage;
    public float speed;
    public float attackRadius;
    public float attackSpeed;
    public float abilityRadius;
    public float abilitySpeed;
    public float rangedFleeRadius;
    public Projectile projectile;
    public float singDrawSpeed;
}
