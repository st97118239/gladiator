using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileObj : MonoBehaviour
{
    public bool isOn;
    public bool isPlayerProj;
    public int dmg;
    public float speed;
    public Vector3 target;
    public EnemyController enemy;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private BoxCollider2D projCollider;
    [SerializeField] private Vector3 spinSpeed;

    private Vector3 moveTo;
    private WaitForSeconds despawnDelay;
    private Vector3 gizmoHitboxScale;
    private Collider2D colliderToIgnore;

    private void Awake()
    {
        if (Application.isPlaying)
            gizmoHitboxScale = projCollider.size * transform.localScale;
    }

    private void Update()
    {
        transform.eulerAngles += spinSpeed;
    }

    public void Load(Projectile givenProj, Vector3 givenTarget, EnemyController givenEnemy, AbilityManager givenAbilityManager, float angle, Collider2D givenColliderToIgnore)
    {
        isOn = true;
        target = givenTarget;
        if (givenEnemy)
        {
            enemy = givenEnemy;
            dmg = enemy.enemy.damage;
            transform.position = enemy.transform.position;
            isPlayerProj = false;
        }
        else
        {
            enemy = null;
            isPlayerProj = true;
            dmg = givenAbilityManager.crossbowDamage;
            transform.position = givenAbilityManager.transform.position;
        }
        speed = givenProj.speed;
        spinSpeed = new Vector3(0, 0, givenProj.spinSpeed);
        spriteRenderer.sprite = givenProj.sprite;
        despawnDelay = new WaitForSeconds(givenProj.despawnDelay);
        transform.eulerAngles = new Vector3(0, 0, angle);
        colliderToIgnore = givenColliderToIgnore;
        Physics2D.IgnoreCollision(projCollider, colliderToIgnore, true);

        gameObject.SetActive(true);
        Vector3 aimDir = (givenTarget - transform.position).normalized;
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
        if (hit.gameObject.CompareTag("Enemy") && isPlayerProj)
        {
            Debug.Log("Hit enemy.");
            hit.gameObject.GetComponent<EnemyController>().Hit(dmg);
            Reset();
        }
        else if (hit.gameObject.CompareTag("Boss") && isPlayerProj)
        {
            Debug.Log("Hit boss.");
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
