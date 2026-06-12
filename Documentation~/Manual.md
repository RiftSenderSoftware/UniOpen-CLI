# UniOpen CLI — Manual

## Architecture

The package is an editor-only assembly (`UniOpenCLI.Editor`) split into four layers:

```
Editor/
├── UniOpenCli.cs                  Public facade: Open(command, dir), pluggable Launcher
├── Core/
│   ├── CliCommand.cs              Serializable command model
│   └── UniOpenCliSettings.cs      Persistence (EditorPrefs) + cached command list
├── Launchers/
│   ├── ITerminalLauncher.cs       Extension point for custom terminals
│   ├── WindowsTerminalLauncher.cs PowerShell, PATH rebuilt from the registry
│   ├── MacTerminalLauncher.cs     Terminal.app via a temp .command script
│   └── LinuxTerminalLauncher.cs   First available emulator (gnome-terminal, konsole, ...)
├── Menu/
│   ├── CliContextMenu.cs          Slot index → command; resolves the clicked asset's folder
│   └── CliMenuGenerator.cs        Code generation of [MenuItem] methods
└── UI/
    ├── UniOpenCliSettingsProvider.cs  Preferences page (reorderable list)
    └── UniOpenCliToolsMenu.cs         Tools → UniOpen CLI shortcuts
```

### Settings (`UniOpenCliSettings`)

The command list is stored in `EditorPrefs` under the key `UniOpenCLI.Commands` as JSON. Settings are local to the developer's machine: each team member configures an individual set of CLIs, and nothing ends up in version control. The key and format are unchanged since 1.0, so upgrades keep existing settings.

### Code generation (`CliMenuGenerator`)

Unity's `[MenuItem]` attribute is resolved at compile time, so menu entries cannot be created at runtime from a list. Instead, applying settings generates `Assets/UniOpenCLI.Generated/CliContextMenu.g.cs` with one menu method per command (plus a small asmdef referencing `UniOpenCLI.Editor`).

The file is written into the **consumer project**, not the package folder, because packages installed through UPM are read-only. On first load (when the file doesn't exist yet) generation happens automatically via `[InitializeOnLoad]`. The folder is safe to add to `.gitignore`.

If the folder gets deleted or corrupted: **Tools → UniOpen CLI → Regenerate Menu Items**.

### Launching (`ITerminalLauncher`)

`UniOpenCli.Open(command, workingDirectory)` resolves the directory to an absolute path and delegates to the platform launcher:

- **Windows** — `powershell.exe -NoExit`. Before running the command, PATH is rebuilt from the registry (`Machine` + `User`), because Unity passes child processes the PATH captured at editor startup — without this, CLIs installed while the editor is open would not be found.
- **macOS** — a temp `.command` script opened with `open`, which runs it in a new Terminal.app window (avoids triple quote-escaping an `osascript` one-liner would need).
- **Linux** — tries gnome-terminal, konsole, xfce4-terminal, x-terminal-emulator, xterm in order; `exec bash` keeps the window open after the command exits.

To integrate a different terminal (Windows Terminal profile, iTerm2, tmux pane, ...), implement `ITerminalLauncher` and assign it in an `[InitializeOnLoad]` class:

```csharp
[InitializeOnLoad]
internal static class MyTerminalSetup
{
    static MyTerminalSetup() => UniOpenCli.Launcher = new MyCustomLauncher();
}
```

## Preferences page

**Edit → Preferences → UniOpen CLI** (searchable: "cli", "terminal", "claude", ...)

| Control | Purpose |
| --- | --- |
| On | Disabled commands keep their menu item, but it is greyed out |
| Name | Menu item label (`/` becomes `-`, duplicates get a suffix) |
| Command | Any shell line: `claude`, `claude --resume`, `git status`, `npm run dev` |
| + / − | Add or remove a command |
| Drag handle | Reorder (order defines menu order) |
| Apply | Save to EditorPrefs and regenerate the menu |
| Revert | Discard unsaved edits |
| Reset to Defaults | Restore Claude / Codex / Gemini (requires Apply) |

Applying triggers a script reload — Unity pauses for a second; that's expected.

## Working directory resolution

- A folder is selected — the terminal opens in it.
- A file is selected — in the file's folder.
- Nothing is selected — in the project root.

## FAQ

**Can I run arbitrary commands, not just AI CLIs?**
Yes — Command is any shell line: `git status`, `npm run dev`, etc.

**The menu didn't appear after installing.**
Wait for compilation to finish. If `Assets/UniOpenCLI.Generated/CliContextMenu.g.cs` is missing, it is created automatically on editor load; or use **Tools → UniOpen CLI → Regenerate Menu Items**.

**How do I fully reset the settings?**
**Reset to Defaults** + **Apply**, or delete the `UniOpenCLI.Commands` key from EditorPrefs.

**Why does Unity recompile when I press Apply?**
Menu items are real generated C# methods — recompilation is what makes them appear.
