using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public Ability[] abilities;
    public int secondarySlot = -1;
    public int dashSlot = -1;
    public int rageSlot = -1;
    public int throwSlot = -1;

    public Ability[] secondaries;
    public Ability[] powers;
    public Ability[] passives;

    public float shieldBlockAmt;
    [SerializeField] private Projectile crossbowProjectile;
    public int crossbowDamage;
    public float crossbowCooldown;
    [SerializeField] private Projectile netProjectile;
    public Projectile netCollapsedProjectile;
    public float netCooldown;
    [SerializeField] private EnemyStateMachine enemyHeld;
    public float throwSpeed;
    public float throwTime;
    public int throwDamage;
    public float throwCooldown;
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    public float rageAtkSpdMultiplier;
    public float rageMovementSpeedMultiplier;
    public float rageTime;
    public float rageCooldown;
    public int lifestealDrainMultiplier;
    public int steadyStanceArmorPoints;
    public float marathonRunnerMovementSpeedMultiplier;

    public bool isBlocking;
    public bool isHoldingEnemy;
    public bool isDashing;
    public int maxPowers;
    public int powersUnlocked;
    public int passivesUnlocked;
    public bool hasLifesteal;

    public bool canUseSecondary;
    public bool canUsePowers;

    public GameManager gameManager;
    public Player player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private LayerMask emptyLayer;
    [SerializeField] private LayerMask dashExcludeLayers;

    private int currentAbilitySlot;
    private float secondaryDelay;
    private float dashDelay;
    private float rageDelay;
    private float throwDelay;
    private InputAction secondaryAction;
    private InputAction aimAction;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        secondarySlot = -1;
        dashSlot = -1;
        rageSlot = -1;
        throwSlot = -1;
        dashDelay = -1;

        aimAction = inputActions.FindAction("Aim");

        if (gameManager)
        {
            foreach (Ability t in gameManager.abilities)
            {
                if (t)
                    NewPower(t, true);
            }
        }
        else
            Debug.LogWarning("No GameManager found.");

        if (shieldBlockAmt > 100)
            Debug.LogWarning("ShieldBlockAmt in AbilityManager should not be above 100.");
        shieldBlockAmt /= 100;
    }

    public void NewPower(Ability newAbility, bool startOfGame)
    {
        if (currentAbilitySlot >= abilities.Length) return;

        SetAbility(newAbility);

        switch (newAbility.abilityType)
        {
            default:
            case AbilityType.None:
                Debug.LogError("No ability selected.");
                break;
            case AbilityType.Shield:
                secondaryAction = inputActions.FindAction("Secondary");
                secondaryAction.performed += ctx => { Shield(); };
                secondaryAction.canceled += ctx => { ShieldCancel(); };
                secondarySlot = currentAbilitySlot;
                secondaryDelay = -1;
                break;
            case AbilityType.Net:
                secondarySlot = currentAbilitySlot;
                secondaryDelay = -1;
                break;
            case AbilityType.Crossbow:
                secondaryAction = inputActions.FindAction("Secondary");
                secondarySlot = currentAbilitySlot;
                secondaryDelay = -1;
                break;
            case AbilityType.Dash:
                dashSlot = currentAbilitySlot;
                dashDelay = -1;
                break;
            case AbilityType.BerserkerRage:
                rageSlot = currentAbilitySlot;
                rageDelay = -1;
                break;
            case AbilityType.Throw:
                throwSlot = currentAbilitySlot;
                throwDelay = -1;
                break;
            case AbilityType.Lifesteal:
                Lifesteal();
                break;
            case AbilityType.SteadyStance:
                SteadyStance();
                break;
            case AbilityType.MarathonRunner:
                MarathonRunner();
                break;
        }

        abilities[currentAbilitySlot] = newAbility;
        uiManager.NewAbility(newAbility, currentAbilitySlot, !startOfGame);
        currentAbilitySlot++;

        if (!startOfGame && PlayerPrefs.GetInt("Tutorial" + newAbility.tutorial.id) != 1)
            StartCoroutine(AbilityTutorial(newAbility.tutorial));
        else
            Time.timeScale = 1;
    }

    private IEnumerator AbilityTutorial(TutorialInfo tutorial)
    {
        yield return null;

        uiManager.ShowTutorial(tutorial);
    }

    private void SetAbility(Ability newAbility)
    {
        switch (newAbility.abilitySort)
        {
            case AbilitySort.Passive:
                passivesUnlocked++;
                break;
            case AbilitySort.Power:
                powersUnlocked++;
                break;
        }
    }

    public void OnSecondary()
    {
        if (!canUseSecondary) return;

        if (isHoldingEnemy)
        {
            ThrowUse();
            return;
        }

        if (secondarySlot == -1 || secondaryDelay >= 0) return;

        switch (abilities[secondarySlot].abilityType)
        {
            case AbilityType.Crossbow:
                Crossbow();
                break;
            case AbilityType.Net:
                Net();
                break;
        }
    }

    public void OnRage()
    {
        if (rageSlot == -1 || !canUsePowers) return;

        BerserkerRage();
    }

    public void OnThrow()
    {
        if (throwSlot == -1 || !canUsePowers) return;

        Throw();
    }

    private void Shield()
    {
        if (!canUseSecondary) return;

        isBlocking = true;
        playerMovement.canMove = false;
        player.canAttack = false;
        player.hasAttackPreview = false;
        player.canHeal = false;
        player.abilityManager.canUsePowers = false;
        player.spriteRenderer.color = player.cooldownColor;
        player.movementScript.rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void ShieldCancel()
    {
        if (!canUseSecondary) return;

        isBlocking = false;
        playerMovement.canMove = true;
        player.canHeal = true;
        player.abilityManager.canUsePowers = true;
        player.hasAttackPreview = true;
        if (player.hasAttackCooldown) return;
        player.canAttack = true;
        player.spriteRenderer.color = player.defaultColor;
        player.movementScript.rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Net()
    {
        if (secondaryDelay >= 0 || !canUseSecondary) return;

        ProjectileObj proj = levelManager.enemyManager.GetProjectile();
        if (!proj)
        {
            Debug.LogError("No projectile object available.");
            return;
        }

        Vector3 aimDir = Vector3.zero;

        if (inputActions.devices.HasValue)
        {
            var device = inputActions.devices.Value[0];

            if (device.name == "Keyboard")
            {
                Vector3 mousePos = player.cam.ScreenToWorldPoint(Input.mousePosition);
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

        proj.Load(netProjectile, aimDir, null, null, this, angle, player.cd2d);
        uiManager.audioManager.PlayPlayerNetThrow();
        StartCoroutine(NetCooldown());
    }

    public void NetCollapse(Vector3 position, float angleZ)
    {
        Net net = levelManager.enemyManager.GetNet();

        if (!net) return;

        net.Load(position, angleZ, this);
    }

    private IEnumerator NetCooldown()
    {
        secondaryDelay = netCooldown;
        uiManager.abilitySlots[secondarySlot].cooldownSlider.value = secondaryDelay / netCooldown;

        while (secondaryDelay > 0)
        {
            yield return null;
            secondaryDelay -= Time.deltaTime;
            uiManager.abilitySlots[secondarySlot].cooldownSlider.value = secondaryDelay / netCooldown;
        }

        secondaryDelay = -1;
    }

    private void Crossbow()
    {
        if (secondaryDelay >= 0 || !canUseSecondary) return;

        ProjectileObj proj = levelManager.enemyManager.GetProjectile();
        if (!proj)
        {
            Debug.LogError("No projectile object available.");
            return;
        }

        Vector3 aimDir = Vector3.zero;

        if (inputActions.devices.HasValue)
        {
            var device = inputActions.devices.Value[0];

            if (device.name == "Keyboard")
            {
                Vector3 mousePos = player.cam.ScreenToWorldPoint(Input.mousePosition);
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

        proj.Load(crossbowProjectile, aimDir, null, null, this, angle, player.cd2d);
        uiManager.audioManager.PlayPlayerBow();
        StartCoroutine(CrossbowCooldown());

    }

    private IEnumerator CrossbowCooldown()
    {
        secondaryDelay = crossbowCooldown;
        uiManager.abilitySlots[secondarySlot].cooldownSlider.value = secondaryDelay / crossbowCooldown;

        while (secondaryDelay > 0)
        {
            yield return null;
            secondaryDelay -= Time.deltaTime;
            uiManager.abilitySlots[secondarySlot].cooldownSlider.value = secondaryDelay / crossbowCooldown;
        }

        secondaryDelay = -1;
    }

    private void OnDash()
    {
        if (dashSlot < 0 || dashDelay >= 0 || !canUseSecondary || !playerMovement.canMove) return;

        Vector2 moveAmount = playerMovement.moveAction.ReadValue<Vector2>();
        if (moveAmount == Vector2.zero) return;

        isDashing = true;
        player.spriteRenderer.color = player.dashColor;
        player.movementScript.rb2d.excludeLayers = dashExcludeLayers;

        playerMovement.rb2d.AddForce(moveAmount * dashSpeed, ForceMode2D.Force);
        playerMovement.rb2d.linearDamping = 5;

        uiManager.audioManager.PlayPlayerDash();
        Invoke(nameof(ResetRigidbody), dashTime);
        StartCoroutine(DashCooldown());
    }

    private void ResetRigidbody()
    {
        playerMovement.rb2d.linearDamping = 10;
        isDashing = false;
        player.spriteRenderer.color = player.defaultColor;
        player.movementScript.rb2d.excludeLayers = emptyLayer;
    }

    private IEnumerator DashCooldown()
    {
        dashDelay = dashCooldown;
        uiManager.abilitySlots[dashSlot].cooldownSlider.value = dashDelay / dashCooldown;

        while (dashDelay > 0)
        {
            yield return null;
            dashDelay -= Time.deltaTime;
            uiManager.abilitySlots[dashSlot].cooldownSlider.value = dashDelay / dashCooldown;
        }

        dashDelay = -1;
    }

    private void BerserkerRage()
    {
        if (rageSlot < 0 || rageDelay >= 0 || !canUsePowers) return;

        player.movementScript.SpeedChange(rageMovementSpeedMultiplier);
        player.MeleeAtkSpeedChange(-rageAtkSpdMultiplier);
        uiManager.audioManager.PlayPlayerRage();

        Invoke(nameof(BerserkerRageTime), rageTime);
        StartCoroutine(RageCooldown());
    }

    private void BerserkerRageTime()
    {
        player.movementScript.SpeedChange(-rageMovementSpeedMultiplier);
        player.MeleeAtkSpeedChange(rageAtkSpdMultiplier);
    }

    private IEnumerator RageCooldown()
    {
        rageDelay = rageCooldown;
        uiManager.abilitySlots[rageSlot].cooldownSlider.value = rageDelay / rageCooldown;

        while (rageDelay > 0)
        {
            yield return null;
            rageDelay -= Time.deltaTime;
            uiManager.abilitySlots[rageSlot].cooldownSlider.value = rageDelay / rageCooldown;
        }

        rageDelay = -1;
    }

    private void Throw()
    {
        if (throwSlot < 0 || throwDelay >= 0 || !canUsePowers || isHoldingEnemy) return;

        Vector3 aimDir = Vector3.zero;

        if (inputActions.devices.HasValue)
        {
            var device = inputActions.devices.Value[0];

            if (device.name == "Keyboard")
            {
                Vector3 mousePos = player.cam.ScreenToWorldPoint(Input.mousePosition);
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
        player.aimTransform.eulerAngles = new Vector3(0, 0, angle);

        List<Collider2D> hitColliders = new();
        Physics2D.OverlapBox(player.meleeWeaponHitbox.position, player.meleeWeaponHitbox.localScale / 2, player.meleeWeaponHitbox.rotation.z, player.filter, hitColliders);

        bool checkIfFirst = true;
        foreach (Collider2D cd2d in hitColliders.OrderBy(e => Vector3.Distance(player.transform.position, e.transform.position)))
        {
            if (!checkIfFirst) break;
            
            if (!cd2d.gameObject.CompareTag("Enemy")) return;
            checkIfFirst = false;
            EnemyStateMachine enemy = cd2d.gameObject.GetComponent<EnemyStateMachine>();
            if (enemy.enemyController.enemy.canBeHeld)
                enemyHeld = enemy;
        }

        if (!enemyHeld) return;

        uiManager.audioManager.PlayPlayerPickUp();
        enemyHeld.GotPickedUp();
        isHoldingEnemy = true;
        canUseSecondary = true;
        uiManager.abilitySlots[throwSlot].cooldownSlider.value = 1;
    }

    private void ThrowUse()
    {
        if (!isHoldingEnemy) return;

        Vector3 aimDir = Vector3.zero;

        if (inputActions.devices.HasValue)
        {
            var device = inputActions.devices.Value[0];

            if (device.name == "Keyboard")
            {
                Vector3 mousePos = player.cam.ScreenToWorldPoint(Input.mousePosition);
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
        player.aimTransform.eulerAngles = new Vector3(0, 0, angle);

        enemyHeld.GotThrown(throwTime, throwDamage);
        enemyHeld.rb2d.AddForce(aimDir * throwSpeed);
        isHoldingEnemy = false;
        enemyHeld = null;
        uiManager.audioManager.PlayPlayerThrow();
        StartCoroutine(ThrowCooldown());
    }

    private IEnumerator ThrowCooldown()
    {
        throwDelay = throwCooldown;
        uiManager.abilitySlots[throwSlot].cooldownSlider.value = throwDelay / throwCooldown;

        while (throwDelay > 0)
        {
            yield return null;
            throwDelay -= Time.deltaTime;
            uiManager.abilitySlots[throwSlot].cooldownSlider.value = throwDelay / throwCooldown;
        }

        throwDelay = -1;
    }

    private void Lifesteal()
    {
        hasLifesteal = true;
    }

    private void SteadyStance()
    {
        player.ArmorPointsChange(steadyStanceArmorPoints);
    }

    private void MarathonRunner()
    {
        player.movementScript.SpeedChange(marathonRunnerMovementSpeedMultiplier);
    }

    public void SaveAbilities()
    {
        if (!gameManager) levelManager.SpawnNewGameManager();

        gameManager.abilities.Clear();

        foreach (Ability ability in abilities)
        {
            gameManager.abilities.Add(ability);
        }
    }
}
