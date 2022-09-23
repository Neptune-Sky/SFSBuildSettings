using System;
using System.Collections.Generic;
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
            return Settings.noAdaptation && !Settings.noAdaptOverride ? false : true;
        }
    }

    [HarmonyPatch(typeof(AdaptModule), "UpdateAdaptation")]
    class FixCucumber
    {
        static bool Prefix()
        {
            return Settings.noAdaptation && !Settings.noAdaptOverride ? false : true;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    class AdaptPartPicker
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            Settings.noAdaptOverride = true;
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            Settings.noAdaptOverride = false;
        }
    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    class KillMagnet
    {
        [HarmonyPrefix]
        static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (Settings.snapping)
            {
                __result = new List<Vector2>();
                return false;
            }

            return true;
        }
    }

    public class Classes
    {
        public static HoldGrid ThisHoldGrid;

        public static BuildGrid ThisBuildGrid;
    }

    [HarmonyPatch(typeof(HoldGrid), "GetSnapPosition_Old")]
    public class HoldGrid_GetSnapPosition_Old
    {
        [HarmonyPrefix]
        public static bool Prefix(Vector2 position, out Vector2 __state)
        {
            __state = position;
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(ref Vector2 __result, Vector2 __state)
        {
            __result = MethodReplacements.MyGetSnapPosition(__state, Classes.ThisHoldGrid);
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "Start")]
    public class HoldGrid_Start
    {
        [HarmonyPrefix]
        public static void Prefix(ref HoldGrid __instance)
        {
            Classes.ThisHoldGrid = __instance;
        }
    }
    [HarmonyPatch(typeof(BuildGrid), "Start")]
    public class BuildGrid_Start
    {
        [HarmonyPrefix]
        public static void Prefix(ref BuildGrid __instance)
        {
            Classes.ThisBuildGrid = __instance;
        }
    }


    [HarmonyPatch(typeof(BuildMenus), nameof(BuildMenus.Rotate))]
    public class CustomRotation
    {
        static void Prefix (ref float rotation)
        {
            if (rotation < 0)
            {
                rotation = -(float)Settings.rotationData.currentVal;
            }
            else
            {
                rotation = (float)Settings.rotationData.currentVal;
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
            if (Settings.windowHolder == null) return;
            float cameraDistance = BuildManager.main.buildCamera.CameraDistance;
            float newWidth = (width * (cameraDistance * 0.12f));
            width = Math.Min(newWidth, 0.1f);
        }
    }
    [HarmonyPatch(typeof(BuildManager), "Awake")]
    public static class PatchZoomLimits
    {
        [HarmonyPrefix]
        public static void Prefix(ref BuildManager __instance)
        {
            __instance.buildCamera.maxCameraDistance = 300;
            __instance.buildCamera.minCameraDistance = 0.1f;
        }
    }
}
