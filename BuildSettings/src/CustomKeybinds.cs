using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS;
using SFS.Audio;
using SFS.UI;
using SFS.World;
using UnityEngine;
using static SFS.Input.KeybindingsPC;
using static BuildSettings.PartModifiers;
// ReSharper disable MemberCanBePrivate.Global

namespace BuildSettings
{
    public class DefaultKeys
    {
        public Key[] CustomRotate = { Key.Ctrl_(KeyCode.Q), Key.Ctrl_(KeyCode.E) };
        public Key ToggleInfiniteArea = KeyCode.I;
        public Key TogglePartClipping = KeyCode.O;
        public Key[] Movement = { KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
        public Key[] ChangeHeight = { Key.Ctrl_(KeyCode.DownArrow), Key.Ctrl_(KeyCode.UpArrow) };
        public Key[] ChangeWidth = { Key.Ctrl_(KeyCode.LeftArrow), Key.Ctrl_(KeyCode.RightArrow) };
        public Key Modifier = KeyCode.LeftShift;
        public Key OrientationMode = KeyCode.LeftAlt;
        public Key[] ChangeValues = { KeyCode.M, KeyCode.Comma };
    }

    public class BS_Keybindings : ModKeybindings
    {
        private static readonly DefaultKeys defaultKeys = new DefaultKeys();

        #region Keys

        public Key[] CustomRotate = defaultKeys.CustomRotate;
        public Key[] Movement = defaultKeys.Movement;
        public Key[] ChangeHeight = defaultKeys.ChangeHeight;
        public Key[] ChangeWidth = defaultKeys.ChangeWidth;
        public Key Modifier = defaultKeys.Modifier;
        public Key OrientationMode = defaultKeys.OrientationMode;
        public Key[] ChangeValues = defaultKeys.ChangeValues;
        public Key ToggleInfiniteArea = defaultKeys.ToggleInfiniteArea;
        public Key TogglePartClipping = defaultKeys.TogglePartClipping;

        #endregion

        public static BS_Keybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<BS_Keybindings>(Main.main);
            SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
        }

        private static void OnBuildLoad()
        {
            AddOnKeyDown_Build(main.CustomRotate[0], () => GUI.CustomRotate(true));
            AddOnKeyDown_Build(main.CustomRotate[1], () => GUI.CustomRotate());
            AddOnKeyDown_Build(main.Movement[0], () => MoveSelectedParts(PartMoveDirection.Down));
            AddOnKeyDown_Build(main.Movement[1], () => MoveSelectedParts(PartMoveDirection.Up));
            AddOnKeyDown_Build(main.Movement[2], () => MoveSelectedParts(PartMoveDirection.Left));
            AddOnKeyDown_Build(main.Movement[3], () => MoveSelectedParts(PartMoveDirection.Right));
            AddOnKeyDown_Build(main.ChangeWidth[0], () => ResizeSelectedParts(PartResizeType.DecreaseWidth));
            AddOnKeyDown_Build(main.ChangeWidth[1], () => ResizeSelectedParts(PartResizeType.IncreaseWidth));
            AddOnKeyDown_Build(main.ChangeHeight[0], () => ResizeSelectedParts(PartResizeType.DecreaseHeight));
            AddOnKeyDown_Build(main.ChangeHeight[1], () => ResizeSelectedParts(PartResizeType.IncreaseHeight));
            AddOnKeyDown_Build(main.ChangeValues[0], SetValues.SetSmallMove);
            AddOnKeyDown_Build(main.ChangeValues[1], SetValues.SetSmallResize);
            AddOnKeyDown_Build(main.Modifier, () =>
            {
                if (!Config.settings.modifierIsToggle) return;
                modifierToggle = !modifierToggle;
                SoundPlayer.main.clickSound.Play();
                MsgDrawer.main.Log("Small Increment: " + modifierToggle);
            });
            AddOnKeyDown_Build(main.OrientationMode, () =>
            {
                if (!Config.settings.orientationIsToggle) return;
                orientationToggle = !orientationToggle;
                SoundPlayer.main.clickSound.Play();
                MsgDrawer.main.Log("Orientation Mode: " + orientationToggle);
            });
            AddOnKeyDown_Build(main.ToggleInfiniteArea, () =>
            {
                if (!Base.worldBase.AllowsCheats)
                {
                    SoundPlayer.main.denySound.Play();
                    return;
                }
                Traverse.Create(SandboxSettings.main).Method("ToggleInfiniteBuildArea").GetValue();
                SoundPlayer.main.clickSound.Play();
                MsgDrawer.main.Log("Infinite Build Area: " +
                                   SandboxSettings.main.settings.infiniteBuildArea);
            });
            AddOnKeyDown_Build(main.TogglePartClipping, () =>
            {
                if (!Base.worldBase.AllowsCheats)
                {
                    SoundPlayer.main.denySound.Play();
                    return;
                }
                Traverse.Create(SandboxSettings.main).Method("TogglePartClipping").GetValue();
                SoundPlayer.main.clickSound.Play();
                MsgDrawer.main.Log("Part Clipping: " + SandboxSettings.main.settings.partClipping);
            });
        }

        public override void CreateUI()
        {
            CreateUI_Text("Build Settings Keybindings");
            CreateUI_Keybinding(CustomRotate, defaultKeys.CustomRotate, "Custom Rotation");
            CreateUI_Space();
            CreateUI_Keybinding(Movement, defaultKeys.Movement, "Move Selected Parts");
            CreateUI_Space();
            CreateUI_Keybinding(ChangeWidth, defaultKeys.ChangeWidth, "Change Selected Part Widths");
            CreateUI_Keybinding(ChangeHeight, defaultKeys.ChangeHeight, "Change Selected Part Heights");
            CreateUI_Space();
            CreateUI_Keybinding(Modifier, defaultKeys.Modifier, "Small Increment Modifier");
            CreateUI_Keybinding(OrientationMode, defaultKeys.OrientationMode, "Orientation Mode");
            CreateUI_Space();
            CreateUI_Keybinding(ChangeValues, defaultKeys.ChangeValues, "Change Small Move/Resize Increment");
            CreateUI_Space();
            CreateUI_Keybinding(ToggleInfiniteArea, defaultKeys.ToggleInfiniteArea, "Toggle Infinite Build Area");
            CreateUI_Keybinding(TogglePartClipping, defaultKeys.TogglePartClipping, "Toggle Part Clipping");
            CreateUI_Space();
        }
    }
}