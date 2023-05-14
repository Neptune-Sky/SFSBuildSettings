using System;
using SFS.IO;
using SFS.UI.ModGUI;
using SFS.Variables;
using TMPro;
using UITools;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
using Type = SFS.UI.ModGUI.Type;

namespace BuildSettings
{
    public class Config : ModSettings<Config.SettingsData>
    {
        protected override FilePath SettingsFile => Main.modFolder.ExtendToFile("Config.txt");

        private static Config main;

        public static void Setup()
        {
            main = new Config();
            main.Initialize();
            ConfigurationMenu.Add("Build Settings", new (string, Func<Transform, GameObject>)[]
            {
                ("Config", transform1 => MenuItems(transform1, ConfigurationMenu.ContentSize))
            });
        }

        public class SettingsData
        {
            public Float_Local windowScale = new() { Value = 0.8f };
            public float smallMove = 0.1f;
            public float smallResize = 0.1f;

            public bool invertKeysByDefault;
            public bool modifierIsToggle;
            public bool orientationIsToggle;
        }

        private static GameObject MenuItems(Transform parent, Vector2Int size)
        {
            Box box = CreateBox(parent, size.x, size.y);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));
            int width = size.x - 60;
            CreateLabel(box, size.x, 50, text: "Build Settings");

            Container scale = CreateContainer(box);
            scale.CreateLayoutGroup(Type.Horizontal, spacing: 0);

            Label label = CreateLabel(scale, width - 225, 32, text: "Window Scale");
            label.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            CreateSlider(scale, 225, settings.windowScale.Value, (0.5f, 1.5f), false,
                val => settings.windowScale.Value = val, val => val.ToPercentString());
            CreateSeparator(box, width - 20);
            CreateToggleWithLabel(box, width, 32, () => settings.invertKeysByDefault,
                () => settings.invertKeysByDefault = !settings.invertKeysByDefault,
                labelText: "Invert Keybinds by Default");
            CreateSeparator(box, width - 20);
            CreateToggleWithLabel(box, width, 32, () => settings.modifierIsToggle, () =>
                {
                    settings.modifierIsToggle = !settings.modifierIsToggle;
                    if (settings.modifierIsToggle == false) PartModifiers.modifierToggle = false;
                },
                labelText: "Modifier Key is Toggle");
            CreateSeparator(box, width - 20);
            CreateToggleWithLabel(box, width, 32, () => settings.orientationIsToggle, () =>
                {
                    settings.orientationIsToggle = !settings.orientationIsToggle;
                    if (settings.orientationIsToggle == false) PartModifiers.orientationToggle = false;
                },
                labelText: "Orientation Key is Toggle");
            return box.gameObject;
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            Application.quitting += onChange;
        }
    }
}
