using System.Collections;
using UnityEngine;

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

    private Vector3 moveTo;
    private WaitForSeconds despawnDelay;
    private Vector3 gizmoHitboxScale;

    private void Awake()
    {
        if (Application.isPlaying)
            gizmoHitboxScale = projCollider.size * transform.localScale;
    }

    public void Load(Projectile givenProj, Vector3 givenTarget, EnemyController givenEnemy, float angle)
    {
        isOn = true;
        target = givenTarget;
        enemy = givenEnemy;
        isPlayerProj = givenProj.playerProj;
        dmg = enemy.enemy.damage;
        speed = givenProj.speed;
        spriteRenderer.sprite = givenProj.sprite;
        despawnDelay = new WaitForSeconds(givenProj.despawnDelay);
        transform.position = enemy.transform.position;
        transform.eulerAngles = new Vector3(0, 0, angle);
        moveTo = Quaternion.Euler(0, 0, angle) * Vector3.up;

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

    //private IEnumerator MoveToTarget()
    //{
    //    while (true)
    //    {
    //        transform.position += moveTo * (Time.deltaTime * speed);
    //        yield return null;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.gameObject.CompareTag("Enemy") && isPlayerProj)
        {
            Debug.Log("Hit enemy.");
            hit.gameObject.GetComponent<EnemyController>().Hit(dmg);
            Reset();
        }
        else if (hit.gameObject.CompareTag("Player") && !isPlayerProj)
        {
            hit.gameObject.GetComponent<Player>().PlayerHit(dmg, true);
            Reset();
        }
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        isOn = false;
        transform.position = Vector3.zero;
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
