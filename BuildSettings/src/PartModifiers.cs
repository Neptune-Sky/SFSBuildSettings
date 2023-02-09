using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SFS.Builds;
using SFS.Parts;
using SFS.Parts.Modules;
using UnityEngine;

namespace BuildSettings
{
    public class PartModifiers 
    {
        public static bool modifierToggle;
        public static bool orientationToggle;

        public enum PartMoveDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        public static void MoveSelectedParts(PartMoveDirection direction)
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();
            Undo.main.RecordStatChangeStep(parts, () =>
            {
                float smallMove = Config.settings.smallMove;

                if ((Input.GetKey(BS_Keybindings.main.Modifier.key) && !Config.settings.modifierIsToggle) || modifierToggle)
                {
                    switch (direction)
                    {
                        case PartMoveDirection.Up:
                            Part_Utility.OffsetPartPosition(new Vector2(0, smallMove), false, parts);
                            break;
                        case PartMoveDirection.Down:
                            Part_Utility.OffsetPartPosition(new Vector2(0, -smallMove), false, parts);
                            break;
                        case PartMoveDirection.Left:
                            Part_Utility.OffsetPartPosition(new Vector2(-smallMove, 0), false, parts);
                            break;
                        case PartMoveDirection.Right:
                            Part_Utility.OffsetPartPosition(new Vector2(smallMove, 0), false, parts);
                            break;
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case PartMoveDirection.Up:
                            Part_Utility.OffsetPartPosition(new Vector2(0, 0.5f), true, parts);
                            break;
                        case PartMoveDirection.Down:
                            Part_Utility.OffsetPartPosition(new Vector2(0, -0.5f), true, parts);
                            break;
                        case PartMoveDirection.Left:
                            Part_Utility.OffsetPartPosition(new Vector2(-0.5f, 0), true, parts);
                            break;
                        case PartMoveDirection.Right:
                            Part_Utility.OffsetPartPosition(new Vector2(0.5f, 0), true, parts);
                            break;
                    }
                }
            });
        }

        public enum PartResizeType
        {
            IncreaseWidth,
            DecreaseWidth,
            IncreaseHeight,
            DecreaseHeight
        }

        public static void ResizeSelectedParts(PartResizeType resize_type)
        {
            Part[] parts = BuildManager.main.holdGrid.selector.selected.ToArray();

            float smallResize = Config.settings.smallResize;

            if ((Input.GetKey(BS_Keybindings.main.Modifier.key) && !Config.settings.modifierIsToggle) || modifierToggle)
            {
                switch (resize_type)
                {
                    case PartResizeType.IncreaseWidth:
                        ResizeParts(new Vector3(smallResize, 0, 0), parts);
                        break;
                    case PartResizeType.DecreaseWidth:
                        ResizeParts(new Vector3(-smallResize, 0, 0), parts);
                        break;
                    case PartResizeType.IncreaseHeight:
                        ResizeParts(new Vector3(0, smallResize, 0), parts);
                        break;
                    case PartResizeType.DecreaseHeight:
                        ResizeParts(new Vector3(0, -smallResize, 0), parts);
                        break;
                }
            }
            else
            {
                switch (resize_type)
                {
                    case PartResizeType.IncreaseWidth:
                        ResizeParts(new Vector3(0.5f, 0, 0), parts);
                        break;
                    case PartResizeType.DecreaseWidth:
                        ResizeParts(new Vector3(-0.5f, 0, 0), parts);
                        break;
                    case PartResizeType.IncreaseHeight:
                        ResizeParts(new Vector3(0, 0.5f, 0), parts);
                        break;
                    case PartResizeType.DecreaseHeight:
                        ResizeParts(new Vector3(0, -0.5f, 0), parts);
                        break;
                }
            }

        }

        static void ResizeParts(Vector3 resizeAmount, Part[] parts)
        {
            bool orientationMode = (Input.GetKey(BS_Keybindings.main.OrientationMode.key) && !Config.settings.orientationIsToggle) || orientationToggle;

            if (orientationMode)
            {
                List<OrientationModule> orientationModules = new List<OrientationModule>();
                foreach (Part part in parts)
                {
                    orientationModules.Add(part.orientation);
                }

                Undo.main.RecordStatChangeStep(orientationModules, () =>
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        Orientation orientation = parts[i].orientation.orientation;
                        orientation.x += resizeAmount.x;
                        orientation.y += resizeAmount.y;
                        parts[i].orientation.ApplyOrientation();
                    }
                });
                return;
            }

            Undo.main.RecordStatChangeStep(parts, () =>
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].variablesModule.doubleVariables.Has("size"))
                    {
                        double size = parts[i].variablesModule.doubleVariables.GetValue("size");
                        double newSize = size + resizeAmount.x;
                        if (newSize <= 0.00001 && resizeAmount.x < 0) continue;
                        parts[i].variablesModule.doubleVariables.SetValue("size", newSize);
                        continue;
                    }

                    double height = parts[i].variablesModule.doubleVariables.GetValue("height");
                    double newHeight = height + resizeAmount.y;
                    parts[i].variablesModule.doubleVariables.SetValue("height", newHeight);

                    if (parts[i].variablesModule.doubleVariables.Has("width"))
                    {
                        double width = parts[i].variablesModule.doubleVariables.GetValue("width");
                        parts[i].variablesModule.doubleVariables.SetValue("width", width + resizeAmount.x);

                        if (parts[i].variablesModule.doubleVariables.Has("width_b")) continue;

                        if (parts[i].variablesModule.doubleVariables.Has("width_original"))
                        {
                            double width_original2 = parts[i].variablesModule.doubleVariables.GetValue("width_original");
                            parts[i].variablesModule.doubleVariables.SetValue("width_original", width_original2 + resizeAmount.x);
                            continue;
                        }
                    }

                    double width_original = parts[i].variablesModule.doubleVariables.GetValue("width_original");
                    double width_upper = parts[i].variablesModule.doubleVariables.GetValue("width_a");
                    double width_lower = parts[i].variablesModule.doubleVariables.GetValue("width_b");

                    double newWidthUpper = width_upper + resizeAmount.x;
                    double newWidthLower = width_lower + resizeAmount.x;
                    // Loosely preserve the final size if the sizes are not equal
                    double newWidthOriginal = Math.Min(newWidthUpper, newWidthLower);

                    parts[i].variablesModule.doubleVariables.SetValue("width_original", newWidthOriginal);
                    parts[i].variablesModule.doubleVariables.SetValue("width_a", newWidthUpper);
                    parts[i].variablesModule.doubleVariables.SetValue("width_b", newWidthLower);

                }
            });

        }

    }

}

