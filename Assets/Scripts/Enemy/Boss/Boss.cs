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
    public int extraAttackDamage;
    public float extraAttackRadius;
    public float extraAttackSpeed;
    public float abilityRadius;
    public float abilityTime;
    public float abilityCooldown;
    public float abilityPower;
    public int abilityUseChance;
    public int swipeAttackChance;
    public float rangedFleeRadius;
    public int rangedPointsToCheck;
    public Projectile projectile;
    public float singDrawSpeed;
    public Enemy enemyToSummon;
    public int enemyAmtToSpawn;
    public bool useAltSlash;
    public int healthPotionChance;
}
