using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniOpenCLI.Editor
{
    public static class CliCommandConfig
    {
        private const string PrefsKey = "UniOpenCLI.Commands";

        public static List<CliCommandData> Load()
        {
            string json = EditorPrefs.GetString(PrefsKey, "");

            if (string.IsNullOrEmpty(json))
                return GetDefaults();

            return JsonUtility.FromJson<CliCommandDataList>(json).Items;
        }

        public static List<CliCommandData> GetDefaults()
        {
            return new List<CliCommandData>
            {
                new() { Name = "Claude", Command = "claude", Enabled = true },
                new() { Name = "Codex", Command = "codex", Enabled = true },
                new() { Name = "Gemini", Command = "gemini", Enabled = false }
            };
        }

        public static void Save(List<CliCommandData> commands)
        {
            var list = new CliCommandDataList { Items = commands };
            string json = JsonUtility.ToJson(list);
            EditorPrefs.SetString(PrefsKey, json);
            CliMenuGenerator.Generate(commands);
        }
    }
}
