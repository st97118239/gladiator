using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    public Ability ability;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Activate(Ability givenAbility)
    {
        ability = givenAbility;
        image.sprite = ability.abilityIcon;
        gameObject.SetActive(true);
    }
}
