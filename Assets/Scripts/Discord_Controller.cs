using UnityEngine;
using Discord;

public class Discord_Controller : MonoBehaviour
{
    private Discord.Discord discord;
    private ActivityManager activityManager;

    private void Awake()
    {
        discord = new Discord.Discord(1427284413942857909, (ulong)Discord.CreateFlags.NoRequireDiscord);
        activityManager = discord.GetActivityManager();
    }

    private void OnDisable()
    {
        discord?.Dispose();
    }

    public void MainMenu()
    {
        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Staring at the Main Menu",
            Assets =
            {
                LargeImage = "logo",
                LargeText = "Main Menu"
            }
        };
    }

    public void Level1()
    {
        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting in the Colosseum",
            Assets =
            {
                LargeImage = "logo",
                LargeText = "Colosseum"
            }
        };
    }

    public void Level2()
    {
        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting in the Forest",
            Assets =
            {
                LargeImage = "logo",
                LargeText = "The Forest"
            }
        };
    }

    public void Level3()
    {
        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting on High Stakes",
            Assets =
            {
                LargeImage = "logo",
                LargeText = "High Stakes"
            }
        };
    }

    public void Level4()
    {
        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting on Olympus",
            Assets =
            {
                LargeImage = "logo",
                LargeText = "Olympus"
            }
        };
    }

    private void Update()
    {
        if (discord == null) return;

        discord.RunCallbacks();
    }
}
