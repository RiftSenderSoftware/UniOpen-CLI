# Changelog

All notable changes to this package are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2.0.0] — 2026-06-12

### Added

- **macOS and Linux support** — Terminal.app via a generated `.command` script; common Linux emulators (gnome-terminal, konsole, xfce4-terminal, x-terminal-emulator, xterm) with automatic fallback.
- **Public scripting API** — `UniOpenCli.Open(command, directory)` and a pluggable `UniOpenCli.Launcher` (`ITerminalLauncher`) for custom terminals.
- **Preferences integration** — settings moved to `Edit → Preferences → UniOpen CLI` with a reorderable command list (drag to reorder, searchable in Preferences).
- `Tools → UniOpen CLI` menu: Settings, Open Terminal at Project Root, Regenerate Menu Items.

### Changed

- **UPM-ready layout** — the repository root is now the package: installable directly via Git URL, `package.json` with full metadata, `Documentation~`, committed `.meta` files.
- Generated menu code moved from the package folder to `Assets/UniOpenCLI.Generated/` (own asmdef), because UPM package installs are read-only. Safe to gitignore — recreated automatically.
- Source restructured into layers: `Core` (model, settings), `Launchers` (per-platform terminal launchers), `Menu` (context menu, codegen), `UI` (settings provider).
- `CliCommandData`/`CliCommandConfig` renamed to `CliCommand`/`UniOpenCliSettings`. The `EditorPrefs` storage format and key are unchanged — existing settings survive the upgrade.

### Removed

- The `Tools → UniOpenCLI` settings window (replaced by the Preferences page).

### Upgrade notes

If you used 1.x as a copied folder, delete the old `Assets/Editor/UniOpenCLI/` (including `Generated/`) before installing 2.0. Your command list is kept — it lives in `EditorPrefs`.

## [1.0.0] — 2026-06-12

### Added

- **Assets → Open CLI** context menu that opens a terminal in the selected asset's folder.
- Settings window **Tools → UniOpenCLI**: add, edit, and disable commands.
- Code generation of menu items from the user's command list.
- Default commands: Claude, Codex, Gemini (disabled).
- PATH rebuilt from the registry on launch — CLIs installed while the editor is running are available without a restart (Windows).
