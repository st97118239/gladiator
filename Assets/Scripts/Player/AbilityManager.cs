using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public Ability[] abilities;
    public int secondary = -1;
    public int ability1 = -1;
    public int ability2 = -1;

    public Ability[] secondaries;
    public Ability[] powers;
    public Ability[] passives;

    public bool isBlocking;
    public float shieldBlockAmt;
    [SerializeField] private Projectile crossbowProjectile;
    public int crossbowDamage;
    public float crossbowCooldown;
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    public float rageAtkSpdMultiplier;
    public int lifestealDrainMultiplier;
    public int steadyStanceArmorPoints;
    public float marathonRunnerMovementSpeedMultiplier;

    public bool hasDash;
    public int dashSlot;
    public int passivesUnlocked;
    public bool hasLifesteal;

    [SerializeField] private Player player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InputActionAsset inputActions;

    private int currentAbilitySlot;
    private float secondaryDelay;
    private float dashDelay;
    private InputAction secondaryAction;
    private InputAction aimAction;

    private void Awake()
    {
        secondary = -1;
        ability1 = -1;
        ability2 = -1;
        dashDelay = -1;

        if (shieldBlockAmt > 100)
            Debug.LogWarning("ShieldBlockAmt in AbilityManager should not be above 100.");
        shieldBlockAmt /= 100;
    }

    public void NewPower(Ability newAbility)
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
                break;
            case AbilityType.Net:
            case AbilityType.Crossbow:
                secondaryAction = inputActions.FindAction("Secondary");
                aimAction = inputActions.FindAction("Aim");
                break;
            case AbilityType.Dash:
                hasDash = true;
                dashSlot = currentAbilitySlot;
                break;
            case AbilityType.BerserkerRage:
            case AbilityType.Throw:
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
        uiManager.NewAbility(newAbility, currentAbilitySlot);
        currentAbilitySlot++;
        secondaryDelay = -1;
        dashDelay = -1;
    }

    private void SetAbility(Ability newAbility)
    {
        switch (newAbility.abilitySort)
        {
            case AbilitySort.Power when ability1 == -1:
                ability1 = currentAbilitySlot;
                break;
            case AbilitySort.Power when ability2 == -1:
                ability2 = currentAbilitySlot;
                break;
            case AbilitySort.Secondary when secondary == -1:
                secondary = currentAbilitySlot;
                break;
            case AbilitySort.Passive:
                passivesUnlocked++;
                break;
        }
    }

    public void OnSecondary()
    {
        if (secondary == -1 || secondaryDelay >= 0) return;

        switch (abilities[secondary].abilityType)
        {
            case AbilityType.Shield:
                //Shield();
                break;
            case AbilityType.Crossbow:
                Crossbow();
                break;
            case AbilityType.Net:
                Net();
                break;
        }
    }

    public void OnAbility1()
    {
        if (ability1 == -1) return;

        switch (abilities[ability1].abilityType)
        {
            case AbilityType.BerserkerRage:
                BerserkerRage();
                break;
            case AbilityType.Throw:
                Throw();
                break;
        }
    }

    public void OnAbility2()
    {
        if (ability2 == -1) return;

        switch (abilities[ability2].abilityType)
        {
            case AbilityType.BerserkerRage:
                BerserkerRage();
                break;
            case AbilityType.Throw:
                Throw();
                break;
        }
    }

    private void Shield()
    {
        Debug.Log("Block");
        isBlocking = true;
        playerMovement.canMove = false;
        player.canAttack = false;
        player.spriteRenderer.color = Color.gray4;
    }

    private void ShieldCancel()
    {
        isBlocking = false;
        playerMovement.canMove = true;

        if (player.hasAttackCooldown) return;
        player.canAttack = true;
        player.spriteRenderer.color = Color.white;
    }

    private void Net()
    {
        Debug.Log("Catch");
    }

    private void Crossbow()
    {
        if (secondaryDelay >= 0) return;

        Debug.Log("Shoot");
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
            }
        }

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;

        proj.Load(crossbowProjectile, aimDir, null, this, angle, player.cd2d);
        StartCoroutine(CrossbowCooldown());

    }

    private IEnumerator CrossbowCooldown()
    {
        secondaryDelay = crossbowCooldown;
        uiManager.abilitySlots[secondary].cooldownSlider.value = secondaryDelay / crossbowCooldown;

        while (secondaryDelay > 0)
        {
            yield return null;
            secondaryDelay -= Time.deltaTime;
            uiManager.abilitySlots[secondary].cooldownSlider.value = secondaryDelay / crossbowCooldown;
        }

        secondaryDelay = -1;
    }

    private void OnDash()
    {
        if (!hasDash || dashDelay >= 0) return;

        Debug.Log("Dash");
        Vector2 moveAmount = playerMovement.moveAction.ReadValue<Vector2>();

        if (playerMovement.canMove)
        {
            playerMovement.rb2d.AddForce(moveAmount * dashSpeed, ForceMode2D.Force);
            playerMovement.rb2d.linearDamping = 5;
        }

        Invoke(nameof(ResetRigidbody), dashTime);
        StartCoroutine(DashCooldown());
    }

    private void ResetRigidbody()
    {
        playerMovement.rb2d.linearDamping = 10;
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
        Debug.Log("Rage");
    }

    private void Throw()
    {
        Debug.Log("Throw");
    }

    private void Lifesteal()
    {
        Debug.Log("Steal health");
        hasLifesteal = true;
    }

    private void SteadyStance()
    {
        Debug.Log("Steady");
        player.ArmorPointsChange(steadyStanceArmorPoints);
    }

    private void MarathonRunner()
    {
        Debug.Log("Run");
        player.movementScript.SpeedChange(marathonRunnerMovementSpeedMultiplier);
    }
}
