using SFS.UI.ModGUI;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Type = SFS.UI.ModGUI.Type;

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

    public class Settings : MonoBehaviour
    {
        public static GameObject windowHolder;
        public static Vector2 gameSize;
        public static Settings inst;

        static readonly int MainWindowID = Builder.GetRandomID();
        static Window window;
        static bool minimized;
        static ButtonWithLabel minButton;

        public static ToggleWithLabel snapToggle;
        public static ToggleWithLabel adaptToggle;
        public static NumberInput gridSnapData;
        public static NumberInput rotationData;

        public static bool snapping;
        public static bool noAdaptation;
        // SFS.UI.ModGUI.Space space;


        public static bool noAdaptOverride;


        void Awake()
        {
            inst = this;
            gridSnapData = CreateData(0.5, 0.0001, 99999);
            rotationData = CreateData(90, 0.0001, 99999);

        }

        NumberInput CreateData(double defaultVal, double min, double max)
        {
            NumberInput ToReturn = new NumberInput
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

        public void ShowGUI()
        {
            windowHolder = Builder.CreateHolder(Builder.SceneToAttach.CurrentScene, "Build Settings");
            var rectTransform = windowHolder.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.position = Vector2.zero;

            window = Builder.CreateWindow(windowHolder.transform, MainWindowID, 375, minimized ? 50 : 400 , 300, 400, true, true, 0.95f, "Build Settings");

            // if (minimized) window.Position = new Vector2(window.Position.x, window.Position.y - 350);

            window.CreateLayoutGroup(Type.Vertical);

            minButton = Builder.CreateButtonWithLabel(window.gameObject.transform, 40, 30, -175, -25, "", minimized ? "+" : "-", Minimize);
            // window.WindowColor = new Color(0.1f, 0.5f, 0.1f);

            Builder.CreateSpace(window, 0, 0);
            snapToggle = Builder.CreateToggleWithLabel(window, 320, 35, () => !snapping, () => snapping = !snapping, 0, 0, "Snap to Parts");
            adaptToggle = Builder.CreateToggleWithLabel(window, 320, 35, () => !noAdaptation, () => noAdaptation = !noAdaptation, 0, 0, "Part Adaptation");

            Box box = Builder.CreateBox(window, 355, 140, 0, 0, 0.75f);
            box.CreateLayoutGroup(Type.Vertical, spacing: 10f);

            Container gridSnapContainer = Builder.CreateContainer(box);
            gridSnapContainer.CreateLayoutGroup(Type.Horizontal, spacing: 10f);

            var gridLabel = Builder.CreateLabel(gridSnapContainer, 200, 35, 0, 0, "Grid Snap");
            Builder.CreateSpace(gridSnapContainer, 20, 0);
            gridSnapData.textInput = Builder.CreateTextInput(gridSnapContainer, 90, 50, 0, 0, gridSnapData.defaultVal.ToString(CultureInfo.InvariantCulture), MakeNumber);

            Container rotationContainer = Builder.CreateContainer(box);
            rotationContainer.CreateLayoutGroup(Type.Horizontal, spacing: 10f);

            Builder.CreateLabel(rotationContainer, 200, 35, 0, 0, "Rotation Degrees");
            Builder.CreateSpace(rotationContainer, 20, 0);
            rotationData.textInput = Builder.CreateTextInput(rotationContainer, 90, 50, 0, 0, rotationData.defaultVal.ToString(CultureInfo.InvariantCulture), MakeNumber);

            Builder.CreateButton(window, 325, 40, 0, 0, Defaults, "Defaults");

            // window.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        }

        void Minimize()
        {
            minimized = !minimized;

            if (!minimized)
            {
                window.Size = new Vector2(375, 400);
                if (window.Position.y < gameSize.y / 3)
                {
                    window.Position = new Vector2(window.Position.x, window.Position.y + 350);
                }
                minButton.button.Text = "-";
            }
            else
            {
                window.Size = new Vector2(375, 50);
                if (window.Position.y < gameSize.y / 3)
                {
                    window.Position = new Vector2(window.Position.x, window.Position.y - 350);
                }
                minButton.button.Text = "+";
            }
            minButton.Position = new Vector2(-175, -25);
        }
        void Defaults()
        {
            snapping = false;
            noAdaptation = false;
            snapToggle.toggle.toggleButton.UpdateUI(false); 
            adaptToggle.toggle.toggleButton.UpdateUI(false);
            gridSnapData.currentVal = gridSnapData.defaultVal;
            gridSnapData.textInput.Text = gridSnapData.defaultVal.ToString(CultureInfo.InvariantCulture);
            rotationData.currentVal = rotationData.defaultVal;
            rotationData.textInput.Text = rotationData.defaultVal.ToString(CultureInfo.InvariantCulture);
        }
        void MakeNumber(string text)
        {
            gridSnapData = Numberify(gridSnapData);
            rotationData = Numberify(rotationData);
        }

        NumberInput Numberify(NumberInput data)
        {
            try
            {
                double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (data.textInput.Text == "." || data.textInput.Text == "")
                {
                    return data;
                }

                data.textInput.Text = data.oldText;
                return data;
            }


            if (data.textInput.Text.Length > 5)
            {
                data.textInput.Text = data.oldText;
            }

            double numCheck = double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);

            if (numCheck == 0)
            {
                data.currentVal = data.defaultVal;
            }
            else if (numCheck < data.min || numCheck > data.max)
            {
                data.currentVal = data.defaultVal;
                data.textInput.Text = data.defaultVal.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                data.currentVal = numCheck.Round(0.00001);
            }

            data.oldText = data.textInput.Text;
            return data;
        }


        Vector2 ClampWindow()
        {
            gameSize = new Vector2((windowHolder.GetComponentInParent<CanvasScaler>().referenceResolution.y / Screen.height) * Screen.width, windowHolder.GetComponentInParent<CanvasScaler>().referenceResolution.y);

            Vector2 pos = window.Position;
            pos.x = Mathf.Clamp(pos.x, window.Size.x / 2, gameSize.x - window.Size.x / 2);
            pos.y = Mathf.Clamp(pos.y, window.Size.y, gameSize.y);
            return pos;
        }


        void Update()
        {
            // gameSize = windowObj.GetComponentInParent<CanvasScaler>().referenceResolution;
            if (windowHolder == null) return;
            window.Position = ClampWindow();
            // gridSnapData.textInput.Size = new Vector2(Mathf.Clamp(70 + 10 * gridSnapData.textInput.Text.Length, 80, 200), 45);
        }
    }
}
