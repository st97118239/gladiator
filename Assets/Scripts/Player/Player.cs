using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerMovement movementScript;
    public InputAction meleeAction;
    public InputAction secondaryAction;
    public int health;
    public float movementSpeed;
    public bool isDead;
    public int meleeDamage;
    public float meleeAtkSpeed;
    private float meleeAtkSpeedMultiplier = 1f;
    public float atkKnockback;

    public bool canAttack;

    public Abilities abilities;
    public Abilities[] activeAbilities = new Abilities[] {};
    private int currentAbilitiySlot = 0;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform meleeWeaponHitbox;
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float meleeHitboxDistanceFromPlayer;
    [SerializeField] private ContactFilter2D filter;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Camera cam;

    private void Start()
    { 
        meleeAction.Enable();
        hpSlider.maxValue = health;
        hpSlider.value = health;
        meleeWeaponHitbox.position += Vector3.up * meleeHitboxDistanceFromPlayer;
        canAttack = true;
        movementScript.canMove = true;
    }

    private void Update()
    {
        meleeAction.started += ctx => { MeleeAttack(); };
        secondaryAction.started += ctx => { SecondaryAction(); };

        levelManager.enemyManger.SetClosest();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AbilityGotten();
        }
    }

    private void MeleeAttack()
    {
        if (!canAttack) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);

        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        List<Collider2D> hitColliders = new();
        Physics2D.OverlapBox(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale / 2, meleeWeaponHitbox.rotation.z, filter, hitColliders);
        
        int i = 0;
        while (i < hitColliders.Count)
        {
            if (hitColliders[i].CompareTag("Enemy"))
                hitColliders[i].GetComponent<EnemyController>().Hit(meleeDamage);
            else if (hitColliders[i].CompareTag("Boss"))
                hitColliders[i].GetComponent<BossController>().Hit(meleeDamage);
            i++;
        }

        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        canAttack = false;

        spriteRenderer.color = Color.gray4;

        yield return new WaitForSeconds(meleeAtkSpeed * meleeAtkSpeedMultiplier);

        spriteRenderer.color = Color.white;

        canAttack = true;
    }
    
    private void SecondaryAction()
    {

    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(cam.ScreenToWorldPoint(Input.mousePosition), transform.position);
        Gizmos.DrawWireCube(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale);
    }

    public void PlayerHit(int damage)
    {
        if (isDead) return;

        health -= damage;
        hpSlider.value = health;

        if (health <= 0)
            PlayerDead();
    }

    private void PlayerDead()
    {
        isDead = true;
        canAttack = false;
        movementScript.canMove = false;
        Debug.Log("Player died.");
        levelManager.GameEnd(true);
    }
    public void AbilityGotten()
    {
        activeAbilities[currentAbilitiySlot] = abilities;
        currentAbilitiySlot += 1;
    }
    public void MeleeAtkSpeedChange(float change)
    {
        meleeAtkSpeedMultiplier += change;
    }
}
