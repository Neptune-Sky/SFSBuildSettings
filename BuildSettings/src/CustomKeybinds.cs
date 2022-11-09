using ModLoader;
using ModLoader.Helpers;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

namespace BuildSettings
{
    public class DefaultKeys
    {
        public Key[] CustomRotate = {
            Key.Ctrl_(KeyCode.Q),
            Key.Ctrl_(KeyCode.E)
        };
    }
    public class BS_Keybindings : ModKeybindings
    {
        static DefaultKeys defaultKeys = new DefaultKeys();

        #region Keys

        Key[] CustomRotate = defaultKeys.CustomRotate;

        #endregion

        static BS_Keybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<BS_Keybindings>(Main.main);

            SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
        }

        static void OnBuildLoad()
        {
            AddOnKeyDown(main.CustomRotate[0], () => GUI.CustomRotate(true));
            AddOnKeyDown(main.CustomRotate[1], () => GUI.CustomRotate());
        }

        public override void CreateUI()
        {
            CreateUI_Text("Build Settings Keybindings");
            CreateUI_Keybinding(CustomRotate, defaultKeys.CustomRotate, "Custom Rotation");
            CreateUI_Space();
        }
    }
}
