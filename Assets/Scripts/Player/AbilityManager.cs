using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Ability[] abilities;
    public Ability ability1;
    public Ability ability2;

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
        if (ability1.abilityType == AbilityType.None)
        {
            ability1 = newAbility;
            player.ability1Action.Enable();
        }
        else if (ability2.abilityType == AbilityType.None)
        {
            ability2 = newAbility;
            player.ability2Action.Enable();
        }
    }

    public void UseSecondary()
    {

    }

    public void UseAbility1()
    {
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

    }

    private void Shield()
    {

    }

    private void Net()
    {

    }

    private void Crossbow()
    {

    }

    private void Dash()
    {

    }

    private void BerserkerRage()
    {

    }

    private void Throw()
    {
        
    }

    private void Lifesteal()
    {

    }

    private void SteadyStance()
    {

    }

    private void MarathonRunner()
    {

    }
}
