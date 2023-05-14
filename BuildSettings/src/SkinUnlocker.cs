using System.Collections.Generic;
using SFS;

namespace BuildSettings
{
    public static class SkinUnlocker
    {
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
    }
}
