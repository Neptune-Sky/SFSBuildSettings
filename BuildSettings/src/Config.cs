using System;
using SFS.IO;
using SFS.Parsers.Json;
using SFS.Variables;
using UnityEngine;

namespace BuildSettings
{
    [Serializable]
    public class SettingsData
    {
        public Float_Local windowScale = new Float_Local { Value = 0.8f };
        public Vector2Int windowPosition = new Vector2Int( 300, 320 );
        public bool invertKeysByDefault;
    }

    public class Config
    {
        static readonly FilePath Path = Main.modFolder.ExtendToFile("Config.txt");

        public static void Load()
        {
            if (!JsonWrapper.TryLoadJson(Path, out data) && Path.FileExists())
            {
                Debug.Log("Config couldn't be loaded correctly, reverting to defaults.");
            }
            data = data ?? new SettingsData();
            Save();
        }

        public static SettingsData data;

        public static void Save()
        {
            if (data == null)
                Load();
            Path.WriteText(JsonWrapper.ToJson(data, true));
        }
    }
}
