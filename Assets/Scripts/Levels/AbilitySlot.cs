using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    public Ability ability;

    private Image image;

    public void Activate(Ability givenAbility)
    {
        image = GetComponent<Image>();
        gameObject.SetActive(true);
        ability = givenAbility;
        image.sprite = ability.abilityIcon;
    }
}
