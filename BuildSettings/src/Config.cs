using System;
using SFS.IO;
using SFS.UI.ModGUI;
using SFS.Variables;
using TMPro;
using UITools;
using UnityEngine;

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
            ConfigurationMenu.Add(null, new (string, Func<Transform, GameObject>)[] { ("Build Settings", MenuItems) });
        }

        public class SettingsData
        {
            public Float_Local windowScale = new Float_Local { Value = 0.8f };
            public bool invertKeysByDefault;
        }

        public static GameObject MenuItems(Transform parent)
        {
            Vector2Int size = ConfigurationMenu.ContentSize;
            Box box = Builder.CreateBox(parent, size.x, size.y, 25);
            box.CreateLayoutGroup(SFS.UI.ModGUI.Type.Vertical, TextAnchor.UpperCenter);
            int width = size.x - 50;

            Builder.CreateLabel(box, size.x, 50, text: "Build Settings");

            Container scale = Builder.CreateContainer(box);
            scale.CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal, spacing: 0);

            Label label = Builder.CreateLabel(scale, width - 250, 35, text: "Window Scale");
            label.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            Builder.CreateSlider(scale, 250, settings.windowScale.Value, (0.5f, 1.5f), false, (val) => settings.windowScale.Value = val, (val) => val.ToPercentString());

            Builder.CreateToggleWithLabel(box, width, 35, () => settings.invertKeysByDefault, () => settings.invertKeysByDefault = !settings.invertKeysByDefault, labelText: "Invert Keybinds by Default");
            return box.gameObject;
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            Application.quitting += onChange;
        }
    }
}
