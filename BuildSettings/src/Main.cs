using System;
using System.IO;
using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS.IO;
using UnityEngine;

namespace BuildSettings
{
    public class Main : Mod
    {

        public override string ModNameID => "BuildSettings";
        public override string DisplayName => "Build Settings";
        public override string Author => "ASoD";
        public override string MinimumGameVersionNecessary => "1.5.8";
        public override string ModVersion => "v1.2";
        public override string Description => "Build settings window and various changes to build mode. See the GitHub repository for a full list of features.";

        public override Action LoadKeybindings => BS_Keybindings.LoadKeybindings;

        public static Settings settings;
        public static Harmony patcher;

        public static Main main;
        public static FolderPath modFolder;

        public override void Early_Load()
        {
            modFolder = new FolderPath(base.ModFolder);

            if (File.Exists(modFolder + "/keybindings.json"))
            {
                File.Delete(modFolder + "/keybindings.json");
            }

            main = this;


            Config.Load();

            patcher = new Harmony("ASoD.BuildSettings.Mod");
            patcher.PatchAll();
        }

        public override void Load()
        {
            SceneHelper.OnBuildSceneLoaded += () =>
            {
                Settings.Setup();
                SkinUnlocker.UnlockSkins();
            };
        }
    }
}
