using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCard : MonoBehaviour
{
    public Ability ability;
    public bool isSelected;

    [SerializeField] private UIManager uiManager;
    [SerializeField] private UIButton button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;

    public void Load(Ability givenAbility)
    {
        ability = givenAbility;
        isSelected = false;

        icon.sprite = ability.abilityIcon;
        nameText.text = ability.abilityName;
    }

    public void Select()
    {
        uiManager.DifferentAbilitySelected();
        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
        button.Deselect();
    }

    public void Reset()
    {
        isSelected = false;
        ability = null;
        button.Reset();
    }
}
