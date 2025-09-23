using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Player player;
    public AbilityManager abilityManager;
    public LevelManager levelManager;
    public EnemyManager enemyManager;
    public Canvas abilityCanvas;

    [SerializeField] private AbilitySlot[] abilitySlots;
    [SerializeField] private AbilityCard[] abilityCards;

    public void ShowAbilityMenu()
    {
        List<Ability> possibleAbilities = new();

        foreach (AbilitySort sort in levelManager.enemyManger.currentWave.abilitySortsToRoll)
        {
            switch (sort)
            {
                default:
                case AbilitySort.None:
                    Debug.LogWarning("No abilities to use.");
                    break;
                case AbilitySort.Secondary:
                    possibleAbilities.AddRange(abilityManager.secondaries);
                    break;
                case AbilitySort.Power:
                    possibleAbilities.AddRange(abilityManager.powers);
                    break;
                case AbilitySort.Passive:
                    possibleAbilities.AddRange(abilityManager.passives);
                    break;
            }
        }

        foreach (AbilityCard card in abilityCards)
        {
            int idx = Random.Range(0, possibleAbilities.Count);

            card.Load(possibleAbilities[idx]);
            possibleAbilities.RemoveAt(idx);
        }

        abilityCanvas.gameObject.SetActive(true);
    }

    public void DifferentAbilitySelected()
    {
        foreach (AbilityCard card in abilityCards)
        {
            card.Deselect();
        }
    }

    public void ConfirmAbility()
    {
        abilityCanvas.gameObject.SetActive(false);

        foreach (AbilityCard card in abilityCards)
        {
            if (card.isSelected)
                abilityManager.NewPower(card.ability);

            card.Reset();
        }
    }

    public void NewAbility(Ability givenAbility, int idx)
    {
        abilitySlots[idx].Activate(givenAbility);

        levelManager.WaveEnd();
    }
}
