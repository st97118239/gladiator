using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    public Ability ability;
    public Slider cooldownSlider;

    private Image image;

    public void Activate(Ability givenAbility)
    {
        cooldownSlider.value = 0;
        image = GetComponent<Image>();
        gameObject.SetActive(true);
        ability = givenAbility;
        image.sprite = ability.abilityIcon;
    }
}
