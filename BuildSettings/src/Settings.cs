using SFS.UI.ModGUI;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Type = SFS.UI.ModGUI.Type;

namespace BuildSettings
{
    public class InputData
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
        public static GameObject windowObj;
        public static Vector2 gameSize;
        public static Settings inst;

        static readonly int MainWindowID = Builder.GetRandomID();
        static Window window;

        public static ToggleWithLabel snapToggle;
        public static ToggleWithLabel adaptToggle;
        public static InputData gridSnapData;
        public static InputData rotationData;

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

        InputData CreateData(double defaultVal, double min, double max)
        {
            InputData ToReturn = new InputData
            {
                textInput = new TextInput(),
                oldText = defaultVal.ToString(),
                defaultVal = defaultVal,
                currentVal = defaultVal,
                min = min,
                max = max
            };
            return ToReturn;
        }

        public void ShowGUI()
        {
            windowObj = Builder.CreateHolder(Builder.SceneToAttach.CurrentScene, "Build Settings");
            windowObj.transform.position = new Vector3(0, 0, 0);

            window = Builder.CreateWindow(windowObj.transform, MainWindowID, 375, 400, 300, 400, true, true, 0.95f, "Build Settings");

            window.CreateLayoutGroup(Type.Vertical);

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
            gridSnapData.textInput = Builder.CreateTextInput(gridSnapContainer, 90, 50, 0, 0, gridSnapData.defaultVal.ToString(), MakeNumber);

            Container rotationContainer = Builder.CreateContainer(box);
            rotationContainer.CreateLayoutGroup(Type.Horizontal, spacing: 10f);

            Builder.CreateLabel(rotationContainer, 200, 35, 0, 0, "Rotation Degrees");
            Builder.CreateSpace(rotationContainer, 20, 0);
            rotationData.textInput = Builder.CreateTextInput(rotationContainer, 90, 50, 0, 0, rotationData.defaultVal.ToString(), MakeNumber);

            Builder.CreateButton(window, 325, 40, 0, 0, Defaults, "Defaults");
        }


        void Defaults()
        {
            snapping = false;
            noAdaptation = false;
            snapToggle.toggle.toggleButton.UpdateUI(false); 
            adaptToggle.toggle.toggleButton.UpdateUI(false);
            gridSnapData.currentVal = gridSnapData.defaultVal;
            gridSnapData.textInput.Text = gridSnapData.defaultVal.ToString();
            rotationData.currentVal = rotationData.defaultVal;
            rotationData.textInput.Text = rotationData.defaultVal.ToString();
        }
        void MakeNumber(string text)
        {
            gridSnapData = Numberify(gridSnapData);
            rotationData = Numberify(rotationData);
        }

        InputData Numberify(InputData data)
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
                data.textInput.Text = data.defaultVal.ToString();
            }
            else
            {
                data.currentVal = numCheck.Round(0.00001);
            }

            data.oldText = data.textInput.Text;
            return data;
        }

        /*
        Vector2 ClampWindow()
        {
            Vector2 pos = window.Position;
            Mathf.Clamp(pos.x, 0 + window.Size.x / 2, gameSize.x - window.Size.x / 2);
            Mathf.Clamp(pos.y, 0 + window.Size.y / 2, gameSize.y - window.Size.y / 2);
            return pos;
        }


        void Update()
        {
            if (windowObj == null) return;
            space.Size.Set(Mathf.Clamp(100 - 10 * gridSnapData.textInput.Text.Length, 0, 70), 0);
            gridSnapData.textInput.Size.Set(Mathf.Clamp(70 + 10 * gridSnapData.textInput.Text.Length, 80, 200), 45);
        }
        */
    }
}
