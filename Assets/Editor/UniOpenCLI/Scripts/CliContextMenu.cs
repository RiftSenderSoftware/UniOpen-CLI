using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace UniOpenCLI.Editor
{
    public static class CliContextMenu
    {
        public static bool IsEnabled(int index)
        {
            var commands = CliCommandConfig.Load();

            if (index < 0 || index >= commands.Count)
                return false;

            return commands[index].Enabled && !string.IsNullOrWhiteSpace(commands[index].Command);
        }

        public static void Open(int index)
        {
            var commands = CliCommandConfig.Load();

            if (index < 0 || index >= commands.Count)
                return;

            var command = commands[index];
            string workingDirectory = GetSelectedDirectory().Replace("'", "''");

            // Unity передаёт дочернему процессу PATH на момент своего запуска,
            // поэтому пересобираем его из реестра — иначе не находятся CLI, установленные после старта Unity
            const string refreshPath =
                "$env:Path = [Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [Environment]::GetEnvironmentVariable('Path','User')";

            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoExit -Command \"{refreshPath}; Set-Location -LiteralPath '{workingDirectory}'; {command.Command}\"",
                UseShellExecute = true
            });
        }

        private static string GetSelectedDirectory()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
                return Directory.GetCurrentDirectory();

            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            return Path.GetFullPath(path);
        }
    }
}
