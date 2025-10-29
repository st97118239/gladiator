using UnityEngine;
using Discord;

public class Discord_Controller : MonoBehaviour
{
    private Discord.Discord discord;
    private ActivityManager activityManager;

    private bool hasDiscordOn;

    private void Awake()
    {
        discord = new Discord.Discord(1427284413942857909, (ulong)Discord.CreateFlags.NoRequireDiscord);

        hasDiscordOn = discord != null;

        if (hasDiscordOn)
            activityManager = discord.GetActivityManager();
    }

    private void OnDisable()
    {
        if (hasDiscordOn)
            discord?.Dispose();
    }

    public void MainMenu()
    {
        if (!hasDiscordOn) return;

        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Staring at the Main Menu",
            Assets =
            {
                LargeImage = "logoalt",
                LargeText = "Main Menu"
            }
        };

        activityManager.UpdateActivity(activity, (res) => { });
    }

    public void Level1()
    {
        if (!hasDiscordOn) return;

        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting in the Colosseum",
            Assets =
            {
                LargeImage = "logoalt",
                LargeText = "Colosseum"
            }
        };

        activityManager.UpdateActivity(activity, (res) => { });
    }

    public void Level2()
    {
        if (!hasDiscordOn) return;

        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting in the Forest",
            Assets =
            {
                LargeImage = "logoalt",
                LargeText = "The Forest"
            }
        };

        activityManager.UpdateActivity(activity, (res) => { });
    }

    public void Level3()
    {
        if (!hasDiscordOn) return;

        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting on High Stakes",
            Assets =
            {
                LargeImage = "logoalt",
                LargeText = "High Stakes"
            }
        };

        activityManager.UpdateActivity(activity, (res) => { });
    }

    public void Level4()
    {
        if (!hasDiscordOn) return;

        Activity activity = new()
        {
            Name = "Gladiator",
            Details = "Fighting on Olympus",
            Assets =
            {
                LargeImage = "logoalt",
                LargeText = "Olympus"
            }
        };

        activityManager.UpdateActivity(activity, (res) => { });
    }

    private void Update()
    {
        discord?.RunCallbacks();
    }
}
