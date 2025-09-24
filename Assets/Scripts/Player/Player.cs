using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerMovement movementScript;
    public int maxHealth;
    public int health;
    public float movementSpeed;
    public bool isDead;
    public int meleeDamage;
    public float meleeAtkSpeed;
    public float atkKnockback;

    public bool canAttack;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform meleeWeaponHitbox;
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float meleeHitboxDistanceFromPlayer;
    [SerializeField] private ContactFilter2D filter;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Camera cam;

    private float atkSpeedMultiplier = 1f;
    private int lifestealDrainMultiplier;
    private int armor;
    private InputAction aimAction;

    private void Awake()
    {
        aimAction = inputActions.FindAction("Aim");
    }

    private void Start()
    { 
        lifestealDrainMultiplier = abilityManager.lifestealDrainMultiplier;
        health = maxHealth;
        hpSlider.maxValue = maxHealth;
        hpSlider.value = health;
        meleeWeaponHitbox.position += Vector3.up * meleeHitboxDistanceFromPlayer;
        canAttack = true;
        movementScript.canMove = true;
    }

    private void Update()
    {
        if (isDead) return;

        levelManager.enemyManger.SetClosest();
    }

    private void OnMelee()
    {
        if (!canAttack || isDead) return;

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

        yield return new WaitForSeconds(meleeAtkSpeed * atkSpeedMultiplier);

        spriteRenderer.color = Color.white;

        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(cam.ScreenToWorldPoint(Input.mousePosition), transform.position);
        Gizmos.DrawWireCube(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale);
    }

    public void PlayerHit(int damage, bool fromEnemy)
    {
        if (isDead) return;

        health -= fromEnemy ? damage - armor : damage;

        if (health > maxHealth) health = maxHealth;

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

    public void MeleeAtkSpeedChange(float change)
    {
        atkSpeedMultiplier += change;
    }

    public void ArmorPointsChange(int change)
    {
        armor += change;
    }

    public void Lifesteal(int healthStolen)
    {
        healthStolen /= lifestealDrainMultiplier;

        PlayerHit(-healthStolen, false);
        
    }
}
