using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Ability> abilities;
    public int healthPotions;
    public int health;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
