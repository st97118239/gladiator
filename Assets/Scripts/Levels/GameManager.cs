using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Ability> abilities;
    public int healthPotions;
    public int health;

    [SerializeField] private int defaultHealth;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void FirstLevel()
    {
        health = defaultHealth;
    }
}
