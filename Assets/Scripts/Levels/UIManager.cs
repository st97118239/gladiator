using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Player player;
    public AbilityManager abilityManager;
    public LevelManager levelManager;
    public EnemyManager enemyManager;

    [SerializeField] private AbilitySlot[] abilitySlots;
    
    public void ShowAbilityMenu()
    {

    }

    public void NewAbility(Ability givenAbility, int idx)
    {
        abilitySlots[idx].Activate(givenAbility);

        levelManager.WaveEnd();
    }
}
