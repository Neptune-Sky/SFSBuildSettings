using System.Collections.Generic;
using SFS;
using SFS.Parts;

namespace BuildSettings
{
    public static class SkinUnlocker
    {
        private static readonly Dictionary<string, string[]> defaultColors = new();
        private static readonly Dictionary<string, string[]> defaultShapes = new();

        public static void Initialize()
        {
            if (defaultColors.Count != 0) return;
            foreach (string key in Base.partsLoader.colorTextures.Keys) defaultColors.Add(key, Base.partsLoader.colorTextures[key].tags);
            foreach (string key in Base.partsLoader.shapeTextures.Keys) defaultShapes.Add(key, Base.partsLoader.shapeTextures[key].tags);
            if (Config.settings.unhideHiddenSkins) UnlockSkins();
        }
        public static void UnlockSkins()
        {
            foreach (string key in Base.partsLoader.colorTextures.Keys)
            {
                    var tags = new List<string>();
                    tags.AddRange(Base.partsLoader.colorTextures[key].tags);
                    if (tags.FindIndex(obj =>  obj != "tank" && obj != "cone" && obj != "fairing" ) == -1)
                    {
                        Base.partsLoader.colorTextures[key].tags = new[] { "tank", "cone", "fairing" };
                    }
            }

            foreach (string key in Base.partsLoader.shapeTextures.Keys)
            {
                var tags = new List<string>();
                tags.AddRange(Base.partsLoader.shapeTextures[key].tags);
                if (tags.FindIndex(obj => obj != "tank" && obj != "cone" && obj != "fairing") == -1)
                {
                    Base.partsLoader.shapeTextures[key].tags = new[] { "tank", "cone", "fairing" };
                }
            }
        }

        public static void LockSkins()
        {
            foreach (string key in defaultColors.Keys)
                Base.partsLoader.colorTextures[key].tags = defaultColors[key];
            foreach (string key in defaultShapes.Keys)
                Base.partsLoader.shapeTextures[key].tags = defaultShapes[key];
        }
    }
}
