using System;
using SFS.IO;
using SFS.UI.ModGUI;
using SFS.Variables;
using TMPro;
using UITools;
using UnityEngine;
using Type = SFS.UI.ModGUI.Type;

namespace BuildSettings
{
    public class Config : ModSettings<Config.SettingsData>
    {
        protected override FilePath SettingsFile => Main.modFolder.ExtendToFile("Config.txt");

        static Config main;

        public static void Setup()
        {
            main = new Config();
            main.Initialize();
            ConfigurationMenu.Add("Build Settings", new (string, Func<Transform, GameObject>)[] { 
            ("Config", transform1 => MenuItems(transform1, ConfigurationMenu.ContentSize)) 
            });
        }

        public class SettingsData
        {
            public Float_Local windowScale = new Float_Local { Value = 0.8f };
            public bool invertKeysByDefault;
        }

        public static GameObject MenuItems(Transform parent, Vector2Int size)
        { 
            Box box = Builder.CreateBox(parent, size.x, size.y);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));
            int width = size.x - 60;

            Builder.CreateLabel(box, size.x, 50, text: "Build Settings");

            Container scale = Builder.CreateContainer(box);
            scale.CreateLayoutGroup(Type.Horizontal, spacing: 0);

            Label label = Builder.CreateLabel(scale, width - 225, 32, text: "Window Scale");
            label.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            Builder.CreateSlider(scale, 225, settings.windowScale.Value, (0.5f, 1.5f), false, (val) => settings.windowScale.Value = val, (val) => val.ToPercentString());

            Builder.CreateSeparator(box, width - 20);

            Builder.CreateToggleWithLabel(box, width, 32, () => settings.invertKeysByDefault, () => settings.invertKeysByDefault = !settings.invertKeysByDefault, labelText: "Invert Keybinds by Default");
            return box.gameObject;
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            Application.quitting += onChange;
        }
    }
}
