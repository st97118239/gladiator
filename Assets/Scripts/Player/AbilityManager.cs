using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Ability[] abilities;
    public Ability secondary;
    public Ability ability1;
    public Ability ability2;

    public Ability[] secondaries;
    public Ability[] powers;
    public Ability[] passives;

    public bool hasLifesteal;

    [SerializeField] private Player player;
    [SerializeField] private UIManager uiManager;

    private int currentAbilitySlot;

    public void NewPower(Ability newAbility)
    {
        if (currentAbilitySlot >= abilities.Length) return;

        switch (newAbility.abilityType)
        {
            default:
            case AbilityType.None:
                Debug.LogError("No ability selected.");
                break;
            case AbilityType.Shield:
            case AbilityType.Net:
            case AbilityType.Crossbow:
            case AbilityType.Dash:
            case AbilityType.BerserkerRage:
            case AbilityType.Throw:
                 SetAbility(newAbility);
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
    }

    private void SetAbility(Ability newAbility)
    {
        switch (newAbility.abilitySort)
        {
            case AbilitySort.Power when !ability1:
                ability1 = newAbility;
                player.ability1Action.Enable();
                break;
            case AbilitySort.Power:
            {
                if (!ability2)
                {
                    ability2 = newAbility;
                    player.ability2Action.Enable();
                }
                break;
            }
            case AbilitySort.Secondary when secondary:
                return;
            case AbilitySort.Secondary:
                secondary = newAbility;
                player.secondaryAction.Enable();
                break;
        }
    }

    public void UseSecondary()
    {
        if (!secondary) return;

        switch (secondary.abilityType)
        {
            case AbilityType.Shield:
                Shield();
                break;
            case AbilityType.Crossbow:
                Crossbow();
                break;
            case AbilityType.Net:
                Net();
                break;
        }
    }

    public void UseAbility1()
    {
        if (!ability1) return;

        switch (ability1.abilityType)
        {
            case AbilityType.Dash:
                Dash();
                break;
            case AbilityType.BerserkerRage:
                BerserkerRage();
                break;
            case AbilityType.Throw:
                Throw();
                break;
        }
    }

    public void UseAbility2()
    {
        if (!ability2) return;

        switch (ability2.abilityType)
        {
            case AbilityType.Dash:
                Dash();
                break;
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
    }

    private void Net()
    {
        Debug.Log("Catch");
    }

    private void Crossbow()
    {
        Debug.Log("Shoot");
    }

    private void Dash()
    {
        Debug.Log("Dash");
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
    }

    private void MarathonRunner()
    {
        Debug.Log("Run");
    }
}
