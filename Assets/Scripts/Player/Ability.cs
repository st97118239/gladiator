using UnityEngine;

[CreateAssetMenu(menuName = "Ability")]
public class Ability : ScriptableObject
{
    public AbilityType abilityType;
    public string abilityName;
    public Sprite abilityIcon;
    public string description;
    public AbilitySort abilitySort;
}
