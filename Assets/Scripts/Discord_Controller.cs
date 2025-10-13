using System;
using UnityEngine;
using Discord;

public class Discord_Controller : MonoBehaviour
{
    [SerializeField] private Sprite logo;

    private Discord.Discord discord;

    private void Start()
    {
        discord = new Discord.Discord(1427284413942857909, (ulong)Discord.CreateFlags.NoRequireDiscord);
        ChangeActivity();
    }

    private void OnDisable()
    {
        discord.Dispose();
    }

    public void ChangeActivity()
    {
        ActivityManager activityManager = discord.GetActivityManager();
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

        activityManager.UpdateActivity(activity, (res) => {Debug.Log("Activity Updated!");});
    }

    private void Update()
    {
        discord.RunCallbacks();
    }
}
