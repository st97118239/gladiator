using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Ability> abilities;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AddAbility(Ability givenAbility)
    {
        abilities.Add(givenAbility);
    }
}
