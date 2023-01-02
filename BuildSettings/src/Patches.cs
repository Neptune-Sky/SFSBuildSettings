using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SFS.Builds;
using SFS.Parts.Modules;
using UnityEngine;

namespace BuildSettings
{
    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    class StopAdaptation
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            return GUI.noAdaptation && !GUI.noAdaptOverride ? false : true;
        }
    }

    [HarmonyPatch(typeof(AdaptModule), "UpdateAdaptation")]
    class FixCucumber
    {
        static bool Prefix()
        {
            return GUI.noAdaptation && !GUI.noAdaptOverride ? false : true;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    class AdaptPartPicker
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            GUI.noAdaptOverride = true;
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            GUI.noAdaptOverride = false;
        }
    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    class KillMagnet
    {
        [HarmonyPrefix]
        static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (GUI.snapping)
            {
                __result = new List<Vector2>();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "GetSnapPosition_Old")]
    static class CustomGridSnap
    {
        public static float GetSnap()
        {
            return GUI.gridSnapData != null ? (float)GUI.gridSnapData.currentVal : 0.5f;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].OperandIs(0.5f))
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, typeof(CustomGridSnap).GetMethod("GetSnap", BindingFlags.Static | BindingFlags.Public));
                }
            }

            return codes.AsEnumerable();
        }
    }


    [HarmonyPatch(typeof(BuildMenus), nameof(BuildMenus.Rotate))]
    public class CustomRotation
    {
        public static bool CustomListener;

        static void Prefix (ref float rotation)
        {

            if (CustomListener)
            {
                CustomListener = false;
                // Debug.Log("Listener Heard: " + rotation.ToString());
                return;
            }

            if (rotation < 0)
            {
                rotation = GUI.GetRotationValue(!GUI.invertKeys, true);
            }
            else
            {
                rotation = GUI.GetRotationValue(!GUI.invertKeys);
            }

        }
    }



    // Makes it so that the outline width for parts shrinks as the camera gets closer
    // Courtesy of Infinity's RandomTweaks
    [HarmonyPatch(typeof(BuildSelector), nameof(BuildSelector.DrawRegionalOutline))]
    public static class SetOutlineWidth
    {
        [HarmonyPrefix]
        public static void DrawRegionalOutline(List<PolygonData> polygons, bool symmetry, Color color, ref float width, float depth = 1f)
        {
            if (GUI.windowHolder == null) return;
            float cameraDistance = BuildManager.main.buildCamera.CameraDistance;
            float newWidth = (width * (cameraDistance * 0.12f));
            width = Math.Min(newWidth, 0.1f);
        }
    }
}
