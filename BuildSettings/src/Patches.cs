using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SFS.Builds;
using SFS.Parts;
using SFS.Parts.Modules;
using UnityEngine;

namespace BuildSettings
{
    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    internal class StopAdaptation
    {
        private static bool Prefix() => !GUI.noAdaptation || GUI.noAdaptOverride;
    }

    [HarmonyPatch(typeof(AdaptModule), "UpdateAdaptation")]
    internal class FixCucumber
    {
        private static bool Prefix() => !GUI.noAdaptation || GUI.noAdaptOverride;
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    internal class AdaptPartPicker
    {
        private static void Prefix() => GUI.noAdaptOverride = true;
        private static void Postfix() => GUI.noAdaptOverride = false;
    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    internal class KillMagnet
    {
        [HarmonyPrefix]
        private static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (!GUI.snapping) return true;
            __result = new List<Vector2>();
            return false;

        }
    }

    [HarmonyPatch(typeof(HoldGrid), "GetSnapPosition_Old")]
    internal static class CustomGridSnap
    {
        public static float GetSnap() => GUI.gridSnapData != null ? (float)GUI.gridSnapData.currentVal : 0.5f;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].OperandIs(0.5f))
                    codes[i] = new CodeInstruction(OpCodes.Call, typeof(CustomGridSnap).GetMethod("GetSnap", BindingFlags.Static | BindingFlags.Public));
            }

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(Part_Utility), nameof(Part_Utility.OffsetPartPosition))]
    internal static class OffsetPartsRoundToGridSnap
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
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
    public static class CustomRotation
    {
        public static bool CustomListener;

        private static void Prefix (ref float rotation)
        {
            if (CustomListener)
            {
                CustomListener = false;
                // Debug.Log("Listener Heard: " + rotation.ToString());
                return;
            }

            rotation = rotation < 0 ? GUI.GetRotationValue(!GUI.invertKeys, true) : GUI.GetRotationValue(!GUI.invertKeys);
        }
    }


    // Makes it so that the outline width for parts shrinks as the camera gets closer
    // Courtesy of Infinity's RandomTweaks
    [HarmonyPatch(typeof(BuildSelector), nameof(BuildSelector.DrawRegionalOutline))]
    public static class SetOutlineWidth
    {
        [HarmonyPrefix]
        [UsedImplicitly]
        public static void DrawRegionalOutline(List<PolygonData> polygons, bool symmetry, Color color, ref float width, float depth = 1f)
        {
            if (GUI.windowHolder == null) return;
            float cameraDistance = BuildManager.main.buildCamera.CameraDistance;
            float newWidth = width * (cameraDistance * 0.12f);
            width = Math.Min(newWidth, 0.1f);
        }
    }
}
