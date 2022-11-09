using System.Collections.Generic;
using System.Linq;
using SFS.Builds;
using SFS.Parts;
using SFS.Parts.Modules;
using SFS.World;
using UnityEngine;
namespace BuildSettings
{
    public class MethodReplacements
    {
        public static Vector2 MyGetSnapPosition(Vector2 position, HoldGrid hold_grid)
        {
            if (GUI.windowHolder == null) return new Vector2();
            hold_grid.holdGrid.transform.position = position;
            ConvexPolygon[] buildColliders = hold_grid.buildGrid.buildColliders.Select((BuildGrid.PartCollider a) => a.colliders).Collapse().ToArray();
            MagnetModule[] modules = hold_grid.holdGrid.partsHolder.GetModules<MagnetModule>();
            MagnetModule[] modules2 = hold_grid.buildGrid.activeGrid.partsHolder.GetModules<MagnetModule>();
            Vector2 vector = Vector2.zero;
            float num = float.PositiveInfinity;
            if (modules.Length <= 8)
            {
                List<Vector2> allSnapOffsets = MagnetModule.GetAllSnapOffsets(modules, modules2, 0.75f);
                if (allSnapOffsets.Count > 0)
                {
                    allSnapOffsets.Sort((Vector2 a, Vector2 b) => a.sqrMagnitude.CompareTo(b.sqrMagnitude));
                    foreach (Vector2 item in allSnapOffsets)
                    {
                        Vector2 transformVector = hold_grid.holdGrid.transform.position;
                        hold_grid.transform.position = position;
                        bool collisionResult = Polygon.Intersect(Part_Utility.GetBuildColliderPolygons(hold_grid.holdGrid.partsHolder.parts.ToArray()).normal, buildColliders, -0.08f);
                        hold_grid.holdGrid.transform.position = transformVector;
                        if (!collisionResult)
                        {
                            return position + item;
                        }
                    }
                }
            }
            if (num < float.PositiveInfinity)
            {
                return position + vector;
            }
            if (SandboxSettings.main.settings.partClipping)
            {
                return position.Round((float)GUI.gridSnapData.currentVal);
            }
            List<Vector2> obj = new List<Vector2>
            {
                position.Round((float)GUI.gridSnapData.currentVal),
                (position + Vector2.left * 0.4f).Round((float)GUI.gridSnapData.currentVal),
                (position + Vector2.right * 0.4f).Round((float)GUI.gridSnapData.currentVal),
                (position + Vector2.up * 0.4f).Round((float)GUI.gridSnapData.currentVal),
                (position + Vector2.down * 0.4f).Round((float)GUI.gridSnapData.currentVal)
            };
            Vector2 result = position;
            float num2 = float.PositiveInfinity;
            foreach (Vector2 item in obj)
            {
                if ((item - position).sqrMagnitude < num2)
                {
                    result = item;
                    num2 = (item - position).sqrMagnitude;
                }
            }
            if (num2 < float.PositiveInfinity)
            {
                return result;
            }
            return position.Round((float)GUI.gridSnapData.currentVal);
        }
    }
}
