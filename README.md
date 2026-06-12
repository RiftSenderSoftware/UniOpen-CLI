# UniOpen CLI

[![Unity](https://img.shields.io/badge/Unity-2021.3%2B-black?logo=unity)](https://unity.com)
[![License](https://img.shields.io/badge/License-Apache--2.0-blue.svg)](LICENSE.md)
[![UPM](https://img.shields.io/badge/UPM-Git%20URL-brightgreen)](#installation)

Open a terminal with your favorite CLI tool — **Claude Code**, **Codex**, **Gemini CLI**, git, anything — straight from the Project window context menu, in the folder of the selected asset.

**Right-click an asset → Open CLI → Claude.** That's it: a terminal opens in that folder with the command already running.

## Features

- 🖱️ **Context menu integration** — `Assets → Open CLI → …` opens a terminal in the selected asset's folder (file → its folder, nothing selected → project root).
- ⚙️ **Fully configurable** — any number of commands with custom names; reorder, disable, or remove them in Preferences.
- 🧩 **Menu items match your list** — entries are code-generated from your configuration, not limited to fixed slots.
- 👤 **Per-developer settings** — stored in `EditorPrefs`; every team member keeps an individual setup, nothing pollutes the repo.
- 🔄 **Fresh PATH on Windows** — PATH is rebuilt from the registry on launch, so CLIs installed while the editor is running are found without a restart.
- 🖥️ **Cross-platform** — PowerShell on Windows, Terminal.app on macOS, common emulators on Linux; pluggable launcher API for anything else.
- 📦 **Zero dependencies**, editor-only assembly, no runtime footprint.

## Installation

### Unity Package Manager (recommended)

`Window → Package Manager → + → Add package from Git URL…`

```
https://github.com/RiftSenderSoftware/UniOpen-CLI.git
```

To pin a version, append a release tag: `…/UniOpen-CLI.git#v2.0.0`.

### Manual

Copy the repository folder anywhere inside your project's `Assets/`.

## Usage

1. Right-click any asset (or empty space) in the Project window.
2. Choose **Open CLI → Claude** (or any command from your list).
3. A terminal opens in the asset's directory with the command running.

Defaults out of the box: **Claude** (`claude`), **Codex** (`codex`), **Gemini** (`gemini`, disabled).

## Configuration

Open **Edit → Preferences → UniOpen CLI** (or **Tools → UniOpen CLI → Settings**):

| Field | Meaning |
| --- | --- |
| On | Disabled commands keep their menu item, but it is greyed out |
| Name | Menu item label |
| Command | Shell command to run — `claude`, `claude --resume`, `git status`, `npm run dev`, … |

Press **Apply** to save and regenerate the menu (Unity recompiles for a second — that's expected). Generated code lives in `Assets/UniOpenCLI.Generated/`; you may add this folder to `.gitignore`, it is recreated automatically.

## Scripting API

```csharp
using UniOpenCLI.Editor;

// Open a terminal at the project root and run a command:
UniOpenCli.Open("git status");

// Open it in a specific folder:
UniOpenCli.Open("claude", "Assets/Scripts");

// Plug in a custom terminal (Windows Terminal profile, iTerm2, tmux, ...):
UniOpenCli.Launcher = new MyCustomLauncher(); // implements ITerminalLauncher
```

## How it works

`[MenuItem]` is a compile-time attribute, so menu entries can't be created at runtime from a list. Instead, UniOpen CLI generates one tiny menu method per command into `Assets/UniOpenCLI.Generated/` (the package itself stays immutable, as UPM requires) and regenerates the file whenever you press Apply. See [Documentation~/Manual.md](Documentation~/Manual.md) for details.

## Package structure

```
UniOpen-CLI/
├── package.json                       UPM manifest
├── README.md · CHANGELOG.md · LICENSE.md
├── Documentation~/
│   └── Manual.md                      Architecture & in-depth guide
└── Editor/
    ├── UniOpenCLI.Editor.asmdef       Editor-only assembly
    ├── UniOpenCli.cs                  Public facade: Open(), pluggable Launcher
    ├── Core/
    │   ├── CliCommand.cs              Serializable command model
    │   └── UniOpenCliSettings.cs      Persistence (EditorPrefs)
    ├── Launchers/
    │   ├── ITerminalLauncher.cs       Extension point for custom terminals
    │   ├── WindowsTerminalLauncher.cs PowerShell + registry PATH refresh
    │   ├── MacTerminalLauncher.cs     Terminal.app
    │   └── LinuxTerminalLauncher.cs   gnome-terminal, konsole, ...
    ├── Menu/
    │   ├── CliContextMenu.cs          Menu slot → command dispatch
    │   └── CliMenuGenerator.cs        [MenuItem] code generation
    └── UI/
        ├── UniOpenCliSettingsProvider.cs  Preferences page
        └── UniOpenCliToolsMenu.cs         Tools → UniOpen CLI shortcuts
```

Generated at runtime in the consumer project (safe to gitignore):

```
Assets/UniOpenCLI.Generated/
├── UniOpenCLI.Generated.asmdef
└── CliContextMenu.g.cs                One [MenuItem] per configured command
```

## Requirements

- Unity **2021.3+**
- Windows, macOS, or Linux

## License

[Apache-2.0](LICENSE.md)
