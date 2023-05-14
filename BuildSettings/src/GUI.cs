using System.Globalization;
using JetBrains.Annotations;
using SFS.Builds;
using SFS.UI.ModGUI;
using UITools;
using UnityEngine;
using UnityEngine.UI;
using static SFS.UI.ModGUI.Builder;
using GUIElement = SFS.UI.ModGUI.GUIElement;

namespace BuildSettings
{
    public class NumberInput
    {
        public TextInput textInput;
        public string oldText;
        public double defaultVal;
        public double currentVal;
        public double min;
        public double max;
    }

    [UsedImplicitly]
    public class GUI
    {
        public static GameObject windowHolder;
        private static Vector2 gameSize;
        public static GUI inst;

        private static readonly int MainWindowID = GetRandomID();
        private static Window window;
        private static bool minimized;
        private static ButtonWithLabel minButton;

        private static ToggleWithLabel snapToggle;
        private static ToggleWithLabel adaptToggle;
        private static ToggleWithLabel invertKeyToggle;
        public static NumberInput gridSnapData;
        private static NumberInput rotationData;

        public static bool snapping;
        public static bool noAdaptation;
        public static bool invertKeys;
        // SFS.UI.ModGUI.Space space;


        public static bool noAdaptOverride;

        public static void Setup()
        {
            gridSnapData = CreateData(0.5, 0.000000000000000000001, 99999);
            rotationData = CreateData(90, 0.000000000000000000001, 99999);

            ShowGUI();
            if (minimized) Minimize(true);
            window.RegisterOnDropListener(OnDragDrop);
            ClampWindow(window);
            Defaults();
            ModSettings<Config.SettingsData>.settings.windowScale.OnChange += Scale;

            BuildManager.main.buildCamera.maxCameraDistance = 300;
            BuildManager.main.buildCamera.minCameraDistance = 0.1f;
        }

        private static NumberInput CreateData(double defaultVal, double min, double max)
        {
            var ToReturn = new NumberInput
            {
                textInput = new TextInput(),
                oldText = defaultVal.ToString(CultureInfo.InvariantCulture),
                defaultVal = defaultVal,
                currentVal = defaultVal,
                min = min,
                max = max
            };
            return ToReturn;
        }

        private static void ShowGUI()
        {

            windowHolder = CreateHolder(SceneToAttach.CurrentScene, "Build Settings");

            window = CreateWindow(windowHolder.transform, MainWindowID, 375, minimized ? 50 : 450, (int)(gameSize.x / 2) - 500, (int)(gameSize.y / 2) - 300, true, false, 0.95f, "Build Settings");

            window.RegisterPermanentSaving("BuildSettings.windowPosition");

            // if (minimized) window.Position = new Vector2(window.Position.x, window.Position.y - 350);

            window.CreateLayoutGroup(Type.Vertical);

            minButton = CreateButtonWithLabel(window.gameObject.transform, 40, 30, -175, -25, "", minimized ? "+" : "-", () => Minimize());
            CreateSpace(window, 0, 0);
            snapToggle = CreateToggleWithLabel(window, 320, 35, () => !snapping, () => snapping = !snapping, 0, 0, "Snap to Parts");
            adaptToggle = CreateToggleWithLabel(window, 320, 35, () => !noAdaptation, () => noAdaptation = !noAdaptation, 0, 0, "Part Adaptation");
            invertKeyToggle = CreateToggleWithLabel(window, 320, 35, () => invertKeys, () => invertKeys = !invertKeys, 0, 0, "Invert Rotate Keybinds");

            Box box = CreateBox(window, 355, 140, 0, 0, 0.75f);
            box.CreateLayoutGroup(Type.Vertical, spacing: 10f);

            Container gridSnapContainer = CreateContainer(box);
            gridSnapContainer.CreateLayoutGroup(Type.Horizontal, spacing: 10f);

            CreateLabel(gridSnapContainer, 200, 35, 0, 0, "Grid Snap");
            CreateSpace(gridSnapContainer, 20, 0);
            gridSnapData.textInput = CreateTextInput(gridSnapContainer, 90, 50, 0, 0, gridSnapData.defaultVal.ToString(CultureInfo.InvariantCulture), MakeNumber);

            Container rotationContainer = CreateContainer(box);
            rotationContainer.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
            CreateLabel(rotationContainer, 200, 35, 0, 0, "Rotation Degrees");
            CreateSpace(rotationContainer, 20, 0);
            rotationData.textInput = CreateTextInput(rotationContainer, 90, 50, 0, 0, rotationData.defaultVal.ToString(CultureInfo.InvariantCulture), MakeNumber);
            CreateButton(window, 325, 40, 0, 0, Defaults, "Defaults");

            window.gameObject.transform.localScale = new Vector3(ModSettings<Config.SettingsData>.settings.windowScale.Value, ModSettings<Config.SettingsData>.settings.windowScale.Value, 1f);
        }

        private static void Minimize(bool setup = false)
        {
            minimized = !minimized;

            if (!minimized)
            {
                window.Size = new Vector2(375, 450);
                if (window.Position.y < gameSize.y / 3 && !setup)
                {
                    window.Position = new Vector2(window.Position.x, window.Position.y + 400 * ModSettings<Config.SettingsData>.settings.windowScale.Value);
                }
                minButton.button.Text = "-";
            }
            else
            {
                window.Size = new Vector2(375, 50);
                if (window.Position.y < gameSize.y / 3)
                {
                    window.Position = new Vector2(window.Position.x, window.Position.y - (400 * ModSettings<Config.SettingsData>.settings.windowScale.Value));
                }

                minButton.button.Text = "+";
            }
            minButton.Position = new Vector2(-175, -25);
        }

        private static void Defaults()
        {
            snapping = false;
            noAdaptation = false;
            invertKeys = Config.settings.invertKeysByDefault;
            snapToggle.toggle.toggleButton.UpdateUI(false);
            adaptToggle.toggle.toggleButton.UpdateUI(false);
            invertKeyToggle.toggle.toggleButton.UpdateUI(false);
            gridSnapData.currentVal = gridSnapData.defaultVal;
            gridSnapData.textInput.Text = gridSnapData.defaultVal.ToString(CultureInfo.InvariantCulture);
            rotationData.currentVal = rotationData.defaultVal;
            rotationData.textInput.Text = rotationData.defaultVal.ToString(CultureInfo.InvariantCulture);
            PartModifiers.modifierToggle = false;
            PartModifiers.orientationToggle = false;
        }

        private static void MakeNumber(string text)
        {
            gridSnapData = Numberify(gridSnapData);
            rotationData = Numberify(rotationData);
        }

        private static void Scale()
        {
            window.gameObject.transform.localScale = new Vector3(ModSettings<Config.SettingsData>.settings.windowScale.Value, ModSettings<Config.SettingsData>.settings.windowScale.Value, 1f);
            ClampWindow(window);
        }

        private static NumberInput Numberify(NumberInput data)
        {
            try
            {
                double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (data.textInput.Text is "." or "")
                    return data;

                data.textInput.Text = data.oldText;
                return data;
            }


            if (data.textInput.Text.Length > 20)
            {
                data.textInput.Text = data.oldText;
            }

            double numCheck = double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);

            if (numCheck == 0)
                data.currentVal = data.defaultVal;
            else if (numCheck < data.min || numCheck > data.max)
            {
                data.currentVal = data.defaultVal;
                data.textInput.Text = data.defaultVal.ToString(CultureInfo.InvariantCulture);
            }
            else
                data.currentVal = numCheck.Round(0.000000000000000000001);

            data.oldText = data.textInput.Text;
            return data;
        }


        private static void ClampWindow(GUIElement input)
        {
            gameSize = new Vector2(windowHolder.GetComponentInParent<CanvasScaler>().referenceResolution.y / Screen.height * Screen.width, windowHolder.GetComponentInParent<CanvasScaler>().referenceResolution.y);

            Vector2 pos = input.Position;
            pos.x = Mathf.Clamp(pos.x, -(gameSize.x / 2) + (ModSettings<Config.SettingsData>.settings.windowScale.Value * window.Size.x / 2), (gameSize.x / 2) - (ModSettings<Config.SettingsData>.settings.windowScale.Value * window.Size.x / 2));
            pos.y = Mathf.Clamp(pos.y, -(gameSize.y / 2) + (window.Size.y * ModSettings<Config.SettingsData>.settings.windowScale.Value), gameSize.y / 2);
            input.Position = pos;
        }


        private static void OnDragDrop()
        {
            if (windowHolder == null) return;
            ClampWindow(window);
        }

        public static float GetRotationValue(bool useCustom, bool negative = false)
        {
            float value = 90;

            if (useCustom)
                value = (float)rotationData.currentVal;

            return negative ? -value : value;
        }

        public static void CustomRotate(bool inverse = false)
        {
            CustomRotation.CustomListener = true;

            float amount = GetRotationValue(invertKeys, !inverse);
            BuildManager.main.buildMenus.Rotate(amount);
        }
    }
}
