using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using ModLoader;
using ModLoader.Helpers;
using SFS.Input;
using SFS.IO;
using SFS.UI;
using UITools;

namespace BuildSettings
{
    [UsedImplicitly]
    public class Main : Mod, IUpdatable
    {
        public override string ModNameID => "BuildSettings";
        public override string DisplayName => "Build Settings";
        public override string Author => "StarMods";
        public override string MinimumGameVersionNecessary => "1.5.10.2";
        public override string ModVersion => "v2.1.5.1";
        public override string Description => "Build settings window and various changes to build mode. See the GitHub repository for a full list of features.";
        public override Dictionary<string, string> Dependencies { get; } = new() { { "UITools", "1.0" } };

        public override Action LoadKeybindings => BS_Keybindings.LoadKeybindings;
        public Dictionary<string, FilePath> UpdatableFiles => new() { { "https://github.com/Neptune-Sky/SFSBuildSettings/releases/latest/download/BuildSettings.dll", new FolderPath(ModFolder).ExtendToFile("BuildSettings.dll") } };

        private static Harmony patcher;

        public static Main main;
        public static FolderPath modFolder;
        private bool bu;

        public override void Early_Load()
        {
            modFolder = new FolderPath(ModFolder);

            main = this;
            bu = NoOldModloader.Test();
            if (!bu) return;

            patcher = new Harmony("ASoD.BuildSettings.Mod");
            patcher.PatchAll();
        }

        public override void Load()
        {
            Config.Setup();
            if (!bu)
            {
                MenuGenerator.OpenConfirmation(CloseMode.Stack, () => "WARNING:\nBuild Settings is not compatible with BuildUpgrade.\n\nAdditionally, the old modloader is no longer maintained and is likely to cause issues with future updates. It is recommended that you discontinue using it.\n\nPlease choose which mod you'd like to keep. (Choosing to keep Build Settings will remove all old modloader components.)", 
                    () => "Build Settings", NoOldModloader.RemoveFiles,
                    () => "BuildUpgrade", () =>
                    {
                        Directory.Delete(ModFolder, true);
                        ApplicationUtility.Relaunch();
                    });
                return;
            }
            SceneHelper.OnBuildSceneLoaded += () =>
            {
                GUI.Setup();
                SkinUnlocker.Initialize();
            };
        }
    }
}
