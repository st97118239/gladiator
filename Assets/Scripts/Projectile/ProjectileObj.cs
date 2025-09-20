using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

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

    private Vector3 moveTo;
    private WaitForSeconds despawnDelay;

    public void Load(Projectile givenProj, Vector3 givenTarget, EnemyController givenEnemy, float angle)
    {
        isOn = true;
        isPlayerProj = givenProj.playerProj;
        dmg = givenProj.damage;
        speed = givenProj.speed;
        spriteRenderer.sprite = givenProj.sprite;
        target = givenTarget;
        enemy = givenEnemy;
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

    private IEnumerator MoveToTarget()
    {
        while (true)
        {
            transform.position += moveTo * (Time.deltaTime * speed);
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.gameObject.CompareTag("Enemy") && isPlayerProj)
        {
            hit.gameObject.GetComponent<EnemyController>().Hit(dmg);
            Reset();
        }
        else if (hit.gameObject.CompareTag("Player") && !isPlayerProj)
        {
            hit.gameObject.GetComponent<Player>().PlayerHit(dmg);
            Reset();
        }
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        isOn = false;
        transform.position = Vector3.zero;
    }
}
