using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Ability> abilities;
    public int healthPotions;
    public int health;

    [SerializeField] private int defaultHealth;

    private Discord_Controller discordController;

    private void Awake()
    {
        discordController = GetComponent<Discord_Controller>();
        DontDestroyOnLoad(gameObject);
    }

    public void Reset()
    {
        abilities.Clear();
        healthPotions = 0;
        health = defaultHealth;
    }

    public void StartLevel(int currentLevel)
    {
        switch (currentLevel)
        {
            case 0:
                discordController.MainMenu();
                break;
            case 1:
                discordController.Level1();
                break;
            case 2:
                discordController.Level2();
                break;
            case 3:
                discordController.Level3();
                break;
            case 4:
                discordController.Level4();
                break;
        }
    }
}
