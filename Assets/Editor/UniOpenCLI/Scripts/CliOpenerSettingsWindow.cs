using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniOpenCLI.Editor
{
    public class CliOpenerSettingsWindow : EditorWindow
    {
        private List<CliCommandData> commands;
        private string savedJson;
        private Vector2 scroll;

        [MenuItem("Tools/UniOpenCLI")]
        private static void Open()
        {
            GetWindow<CliOpenerSettingsWindow>("UniOpenCLI");
        }

        private void OnEnable()
        {
            commands = CliCommandConfig.Load();
            savedJson = ToJson(commands);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("CLI Commands", EditorStyles.boldLabel);

            bool isDirty = ToJson(commands) != savedJson;
            titleContent.text = isDirty ? "UniOpenCLI*" : "UniOpenCLI";

            var prevColor = GUI.color;
            GUI.color = isDirty ? new Color(1f, 0.65f, 0.2f) : Color.green;
            EditorGUILayout.LabelField(isDirty ? "● Unsaved changes" : "● Saved", EditorStyles.boldLabel);
            GUI.color = prevColor;

            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (int i = 0; i < commands.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");

                commands[i].Enabled = EditorGUILayout.Toggle("Enabled", commands[i].Enabled);
                commands[i].Name = EditorGUILayout.TextField("Name", commands[i].Name);
                commands[i].Command = EditorGUILayout.TextField("Command", commands[i].Command);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Remove"))
                {
                    commands.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add CLI"))
            {
                commands.Add(new CliCommandData
                {
                    Name = "New CLI",
                    Command = "",
                    Enabled = true
                });
            }

            using (new EditorGUI.DisabledScope(!isDirty))
            {
                if (GUILayout.Button("Save"))
                {
                    Save();
                }

                if (GUILayout.Button("Discard Changes"))
                {
                    commands = CliCommandConfig.Load();
                    GUI.FocusControl(null);
                }
            }

            if (GUILayout.Button("Reset to Defaults"))
            {
                commands = CliCommandConfig.GetDefaults();
                GUI.FocusControl(null);
            }
        }

        private void Save()
        {
            CliCommandConfig.Save(commands);
            savedJson = ToJson(commands);
            AssetDatabase.Refresh();
        }

        private static string ToJson(List<CliCommandData> commands)
        {
            return JsonUtility.ToJson(new CliCommandDataList { Items = commands });
        }
    }
}
