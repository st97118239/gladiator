using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Camera cam;
    public PlayerMovement movementScript;
    public AbilityManager abilityManager;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D cd2d;
    public int maxHealth;
    public int health;
    public float movementSpeed;
    public bool isDead;
    public int meleeDamage;
    public float meleeAtkSpeed;
    public float atkKnockback;
    public int healthPotionHealAmt;
    public int healthPotionCap;
    public float hitTime;

    public bool canAttack;
    public bool hasAttackCooldown;
    public bool hasAttackPreview;
    public bool canHeal;

    public bool isLookingRight;

    public Color defaultColor;
    public Color cooldownColor;
    public Color hitColor;
    public Color dashColor;

    public Platform currentPlatform;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UIManager uiManager;
    public Transform meleeWeaponHitbox;
    public Transform aimTransform;
    [SerializeField] private SpriteRenderer aimPreview;
    [SerializeField] private Color aimPreviewColor;
    [SerializeField] private Color disabledAimPreviewColor;
    [SerializeField] private float meleeHitboxDistanceFromPlayer;
    public ContactFilter2D filter;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Animator slashAnimator;

    private int healthPotions;
    private float atkSpeedMultiplier = 1f;
    private int lifestealDrainMultiplier;
    private int armor;
    private InputAction aimAction;
    private Vector3 gizmoHitboxScale;

    private static readonly int MeleeSlash = Animator.StringToHash("MeleeSlash");

    private void Awake()
    {
        health = maxHealth;

        if (abilityManager.gameManager)
        {
            health = abilityManager.gameManager.health;

            for (int i = 0; i < abilityManager.gameManager.healthPotions; i++) 
                GetHealthPotion();
        }
        else
            Debug.LogWarning("No GameManager found.");
        
        aimAction = inputActions.FindAction("Aim");
        if (Application.isPlaying)
            gizmoHitboxScale = cd2d.size * transform.localScale;
    }

    private void Start()
    {
        uiManager.UpdateHealthPotions(healthPotions);
        lifestealDrainMultiplier = abilityManager.lifestealDrainMultiplier;
        hpSlider.maxValue = maxHealth;
        hpSlider.value = health;
        meleeWeaponHitbox.position += Vector3.up * meleeHitboxDistanceFromPlayer;
        canAttack = true;
        hasAttackPreview = true;
        movementScript.canMove = true;
        canHeal = true;
        abilityManager.canUseSecondary = true;
        abilityManager.canUsePowers = true;
        uiManager.canPause = true;
    }

    private void Update()
    {
        if (isDead) return;

        levelManager.enemyManager.SetClosest();

        if (!hasAttackPreview || Time.timeScale == 0) return;

        Vector3 aimDir = Vector3.zero;

        if (inputActions.devices.HasValue)
        {
            var device = inputActions.devices.Value[0];

            if (device.name == "Keyboard")
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(mousePos.x, mousePos.y, 0);
                aimDir = (mousePos - transform.position).normalized;
            }
            else
            {
                aimDir = aimAction.ReadValue<Vector2>();
            }
        }

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;

        if (aimDir == Vector3.zero)
        {
            aimPreview.color = Color.clear;
            return;
        }

        aimPreview.color = hasAttackCooldown ? disabledAimPreviewColor : aimPreviewColor;

        aimTransform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void OnPause()
    {
        levelManager.uiManager.PauseMenu();
    }

    public void GetHealthPotion()
    {
        if (healthPotions >= healthPotionCap) return;

        healthPotions++;
        uiManager.UpdateHealthPotions(healthPotions);
    }

    private void OnHeal()
    {
        if (healthPotions <= 0 || health >= maxHealth) return;

        uiManager.audioManager.PlayPlayerPotionUse();
        healthPotions--;

        PlayerHit(-healthPotionHealAmt, false, true);

        uiManager.UpdateHealthPotions(healthPotions);
    }

    private void OnMelee()
    {
        if (!canAttack || isDead) return;

        Vector3 aimDir = Vector3.zero;

        if (inputActions.devices.HasValue)
        {
            var device = inputActions.devices.Value[0];

            if (device.name == "Keyboard")
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(mousePos.x, mousePos.y, 0);
                aimDir = (mousePos - transform.position).normalized;
            }
            else
            {
                aimDir = aimAction.ReadValue<Vector2>();
                if (aimDir == Vector3.zero) return;
            }
        }

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        slashAnimator.SetTrigger(MeleeSlash);

        List<Collider2D> hitColliders = new();
        Physics2D.OverlapBox(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale / 2, meleeWeaponHitbox.rotation.z, filter, hitColliders);
        
        int i = 0;
        while (i < hitColliders.Count)
        {
            if (hitColliders[i].CompareTag("Enemy"))
                hitColliders[i].GetComponent<EnemyController>().Hit(meleeDamage, true);
            else if (hitColliders[i].CompareTag("Boss"))
                hitColliders[i].GetComponent<BossController>().Hit(meleeDamage, false, true);
            i++;
        }

        uiManager.audioManager.PlayPlayerMelee();
        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        canAttack = false;
        hasAttackCooldown = true;
        aimPreview.color = disabledAimPreviewColor;

        spriteRenderer.color = cooldownColor;

        float timer = meleeAtkSpeed * atkSpeedMultiplier;

        for (float i = 0; i < timer + Time.deltaTime; i += Time.deltaTime)
        {
            if (Time.timeScale != 0)
                yield return null;
        }

        hasAttackCooldown = false;

        if (abilityManager.isBlocking) yield break;

        canAttack = true;
        spriteRenderer.color = defaultColor;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, gizmoHitboxScale);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(cam.ScreenToWorldPoint(Input.mousePosition), transform.position);
        Gizmos.DrawWireCube(meleeWeaponHitbox.position, meleeWeaponHitbox.localScale);
    }

    public void PlayerHit(int damage, bool fromEnemy, bool forced)
    {
        if (isDead || (abilityManager.isDashing && !forced)) return;

        float dmgToDo = damage;

        if (fromEnemy)
        {
            if (abilityManager.isBlocking)
                dmgToDo -= dmgToDo * abilityManager.shieldBlockAmt;

            dmgToDo -= armor;

            if (dmgToDo < 0)
                dmgToDo = 0;
        }
        
        damage = Mathf.RoundToInt(dmgToDo);

        health -= damage;
        if (abilityManager.isBlocking && fromEnemy)
            uiManager.audioManager.PlayPlayerBlock();
        else
            uiManager.audioManager.PlayPlayerHit();


        if (health > maxHealth) health = maxHealth;

        hpSlider.value = health;

        if (health <= 0)
            PlayerDead();
    }

    private void PlayerDead()
    {
        isDead = true;
        canAttack = false;
        hasAttackPreview = false;
        movementScript.canMove = false;
        canHeal = false;
        abilityManager.canUseSecondary = false;
        abilityManager.canUsePowers = false;
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

        PlayerHit(-healthStolen, false, true);
        
    }

    public void SavePotions()
    { 
        abilityManager.gameManager.healthPotions = healthPotions;
        abilityManager.gameManager.health = health;
    }
}
