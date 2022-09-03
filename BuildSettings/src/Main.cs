using ModLoader;
using ModLoader.Helpers;
using HarmonyLib;
using UnityEngine;

namespace BuildSettings
{
    public class Main : Mod
    {

        public override string ModNameID => "BuildSettings";
        public override string DisplayName => "Build Settings";
        public override string Author => "ASoD";
        public override string MinimumGameVersionNecessary => "1.5.7";
        public override string ModVersion => "v1.0";
        public override string Description => "";

        public static GameObject settings;
        public static Harmony patcher;

        public override void Early_Load()
        {
            settings = new GameObject("BuildSettings", typeof(Settings));
            Object.DontDestroyOnLoad(settings);
            
            patcher = new Harmony("ASoD.BuildSettings.Mod");
            patcher.PatchAll();
        }

        public override void Load()
        {
            SceneHelper.OnBuildSceneLoaded += () =>
            {
                Settings.inst.ShowGUI();
                SkinUnlocker.UnlockSkins();
            };
        }
    }
}
