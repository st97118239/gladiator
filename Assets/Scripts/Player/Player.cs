using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public InputAction meleeAction;
    public int health;
    public float speed;
    public float movementSpeed;
    public bool isDead;
    public int meleeDamage;

    [SerializeField] private Transform meleeWeaponHitbox;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private ContactFilter2D filter;

    [SerializeField] private Slider hpSlider;

    private void Start()
    {
        meleeAction.Enable();
        hpSlider.maxValue = health;
        hpSlider.value = health;
    }

    private void Update()
    {
        speed = movementSpeed * Time.deltaTime;

        meleeAction.started += ctx => { MeleeAttack(); };
    }

    private void MeleeAttack()
    {
        Debug.Log("LMB");

        List<Collider2D> hitColliders = new();
        Physics2D.OverlapBox(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale / 2, meleeWeaponHitbox.rotation.z, filter, hitColliders);
        
        int i = 0;
        while (i < hitColliders.Count)
        {
            Debug.Log("Hit : " + hitColliders[i].name + i);
            hitColliders[i].GetComponent<EnemyController>().Hit(meleeDamage);
            i++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (Application.isPlaying)
            Gizmos.DrawWireCube(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale);
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
