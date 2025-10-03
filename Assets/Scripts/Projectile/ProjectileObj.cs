using System.Collections;
using UnityEngine;

public class ProjectileObj : MonoBehaviour
{
    public bool isOn;
    public bool isPlayerProj;
    public bool isNet;
    public int dmg;
    public float speed;
    public Vector3 target;
    public EnemyController enemy;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private BoxCollider2D projCollider;
    [SerializeField] private Vector3 spinSpeed;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask enemyLayer;

    private Vector3 moveTo;
    private WaitForSeconds despawnDelay;
    private Vector3 gizmoHitboxScale;
    private Collider2D colliderToIgnore;
    private AbilityManager abilityManager;

    private void Awake()
    {
        if (Application.isPlaying)
            gizmoHitboxScale = projCollider.size * transform.localScale;
    }

    private void Update()
    {
        transform.eulerAngles += spinSpeed * Time.deltaTime;
    }

    public void Load(Projectile givenProj, Vector3 givenTarget, EnemyController givenEnemy, AbilityManager givenAbilityManager, float angle, Collider2D givenColliderToIgnore)
    {
        isOn = true;
        Vector3 aimDir;
        if (givenEnemy)
        {
            enemy = givenEnemy;
            dmg = enemy.enemy.damage;
            transform.position = enemy.transform.position;
            isPlayerProj = false;
            rigid.excludeLayers = enemyLayer;
            target = givenTarget;
            aimDir = (givenTarget - transform.position).normalized;
        }
        else
        {
            enemy = null;
            isPlayerProj = true;
            abilityManager = givenAbilityManager;
            dmg = abilityManager.crossbowDamage;
            transform.position = abilityManager.transform.position;
            rigid.excludeLayers = playerLayer;
            aimDir = givenTarget;
        }
        isNet = givenProj.isNet;
        speed = givenProj.speed;
        spinSpeed = new Vector3(0, 0, givenProj.spinSpeed);
        spriteRenderer.sprite = givenProj.sprite;
        despawnDelay = new WaitForSeconds(givenProj.despawnDelay);
        transform.eulerAngles = new Vector3(0, 0, angle);
        colliderToIgnore = givenColliderToIgnore;
        Physics2D.IgnoreCollision(projCollider, colliderToIgnore, true);

        gameObject.SetActive(true);
       
        rigid.AddForce(aimDir * speed);

        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return despawnDelay;

        Reset();
    }

    private void OnCollisionEnter2D(Collision2D hit)
    {
        if ((hit.gameObject.CompareTag("Enemy") && isNet) || (hit.gameObject.CompareTag("Boss") && isNet))
        {
            abilityManager.NetCollapse(hit.transform.position, transform.rotation.z);
            Reset();
        }
        else if (hit.gameObject.CompareTag("Enemy") && isPlayerProj)
        {
            hit.gameObject.GetComponent<EnemyController>().Hit(dmg);
            Reset();
        }
        else if (hit.gameObject.CompareTag("Boss") && isPlayerProj)
        {
            hit.gameObject.GetComponent<BossController>().Hit(dmg);
            Reset();
        }
        else if (hit.gameObject.CompareTag("Player") && !isPlayerProj)
        {
            hit.gameObject.GetComponent<Player>().PlayerHit(dmg, true);
            Reset();
        }
        else if (hit.gameObject.CompareTag("Wall"))
            Reset();
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        isOn = false;
        transform.position = Vector3.zero;
        Physics2D.IgnoreCollision(projCollider, colliderToIgnore, false);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        //Gizmos.color = Color.darkBlue;
        //Gizmos.DrawLine(transform.position, enemyController.enemyManager.player.transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, gizmoHitboxScale);
    }
}
