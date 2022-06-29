using MelonLoader;
using System;
using Discord;
using System.Threading;

[assembly: MelonInfo(typeof(DiscordStatus.DiscordStatusPlugin), "Discord Status for 20MTD", "1.0.0", "SlidyDev (forked by samk0)")]
[assembly: MelonColor(ConsoleColor.DarkCyan)]
[assembly: MelonGame(null, null)]

namespace DiscordStatus
{
    public class DiscordStatusPlugin : MelonMod
    {
        public const long AppId = 991695489370173490;
        public Discord.Discord discordClient;
        public ActivityManager activityManager;
        public string genericStateDisplay = "v. 1.0.0";

        private bool gameClosing;
        public bool GameStarted { get; private set; }
        public long gameStartedTime;

        public override void OnApplicationStart()
        {
            DiscordLibraryLoader.LoadLibrary();
            InitializeDiscord();
            //UpdateActivity();
            new Thread(DiscordLoopThread).Start();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            string details = "loading";

            switch(sceneName)
            {
                case "TitleScreen":
                    details = "In Menu";
                    break;

                case "Battle":
                    details = "Surviving";
                    break;

            }

            UpdateActivity(details, genericStateDisplay);
        }

        public override void OnApplicationLateStart()
        {
            GameStarted = true;
            gameStartedTime = getTimestamp();

           // UpdateActivity();
        }

        public override void OnApplicationQuit()
        {
            gameClosing = true;
        }

        public void DiscordLoopThread()
        {
            for (; ; )
            {
                if (gameClosing)
                    break;

                discordClient.RunCallbacks();
                Thread.Sleep(200);
            }
        }

        public void InitializeDiscord()
        {
            discordClient = new Discord.Discord(AppId, (ulong)CreateFlags.NoRequireDiscord);
            discordClient.SetLogHook(LogLevel.Debug, DiscordLogHandler);

            activityManager = discordClient.GetActivityManager();
        }

        private void DiscordLogHandler(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                case LogLevel.Debug:
                    LoggerInstance.Msg(message);
                    break;

                case LogLevel.Warn:
                    LoggerInstance.Warning(message);
                    break;

                case LogLevel.Error:
                    LoggerInstance.Error(message);
                    break;
            }
        }

        public void UpdateActivity(string details, string state)
        {
            var activity = new Activity
            {
                Details = details
            };

            activity.Assets.LargeImage = "lilith";
            activity.Name = $"20MTD";
            activity.Instance = true;
            activity.Assets.LargeText = activity.Name;
       
            activity.State = state;

            if (GameStarted)
                activity.Timestamps.Start = getTimestamp();

            activityManager.UpdateActivity(activity, ResultHandler);
        }

        public void ResultHandler(Result result)
        {

        }

        public long getTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
