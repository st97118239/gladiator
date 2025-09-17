using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public InputAction meleeAction;
    public int health;
    public float movementSpeed;
    public bool isDead;
    public int meleeDamage;
    public float meleeAtkSpeed;

    [SerializeField] private Transform meleeWeaponHitbox;
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float meleeHitboxDistanceFromPlayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private ContactFilter2D filter;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Camera cam;

    private bool canAttack;

    private void Start()
    {
        meleeAction.Enable();
        hpSlider.maxValue = health;
        hpSlider.value = health;
        meleeWeaponHitbox.position += Vector3.up * meleeHitboxDistanceFromPlayer;
        canAttack = true;
    }

    private void Update()
    {
        meleeAction.started += ctx => { MeleeAttack(); };
    }

    private void MeleeAttack()
    {
        if (!canAttack) return;

        Debug.Log("LMB");

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);

        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        Debug.Log(angle);

        List<Collider2D> hitColliders = new();
        Physics2D.OverlapBox(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale / 2, meleeWeaponHitbox.rotation.z, filter, hitColliders);
        
        int i = 0;
        while (i < hitColliders.Count)
        {
            Debug.Log("Hit : " + hitColliders[i].name + i);
            hitColliders[i].GetComponent<EnemyController>().Hit(meleeDamage);
            i++;
        }

        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        canAttack = false;

        WaitForSeconds wait = new(meleeAtkSpeed);

        yield return wait;

        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(cam.ScreenToWorldPoint(Input.mousePosition), transform.position);
    }

    public void PlayerHit(int damage)
    {
        health -= damage;
        hpSlider.value = health;

        if (health <= 0)
        {
            isDead = true;
        }
    }
}
