using System.IO;
using UnityEngine.Device;

namespace BuildSettings
{
    public static class NoOldModloader
    {
        private static readonly char s = Path.DirectorySeparatorChar;
        public static bool Test()
        {
            return !File.Exists(Application.dataPath + $"{s}Mods{s}BuildUpgrade{s}BuildUpgrade.dll") || !File.Exists(Application.dataPath + $"{s}Managed{s}ModLoader.dll");
        }

        public static void RemoveFiles()
        {
            Directory.Delete(Application.dataPath + $"{s}Mods", true);
            File.Delete(Application.dataPath + $"{s}Managed{s}ModLoader.dll");
            ApplicationUtility.Relaunch();
        }
    }
}