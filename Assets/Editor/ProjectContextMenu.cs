using UnityEditor;
using UnityEngine;

public static class ProjectContextMenu
{
    [MenuItem("Assets/Open Console")]
    private static void OpenConsole()
    {
        EditorWindow.GetWindow(
            System.Type.GetType("UnityEditor.ConsoleWindow,UnityEditor")
        );
    }
}