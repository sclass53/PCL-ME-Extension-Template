<h1 align="center">PCL-ME Extension Template</h1>

<p align="center">
  Plugin Template for PCL-ME.
</p>

<p align="center">
  <a href="https://github.com/TheUnknownThing/PCL-ME">
    <img src="https://img.shields.io/badge/PCL--ME-EXTENSION-green?style=for-the-badge" alt="PCL-ME">
  </a>
  <a href="https://www.gnu.org/licenses/gpl-3.0">
    <img src="https://img.shields.io/badge/License-GPLv3-blue.svg?style=for-the-badge" alt="License: GPL v3">
  </a>
</p>

An annotated starter project for a PCL-ME extension. It is based on the official
`examples/HelloExtension` sample and is meant to be copied
and edited, not published as-is. You can refer to the docs of 
[PCL-ME](https://github.com/TheUnknownThing/PCL-ME) for more details.

> This is a **template**, not a demo. The extension id is the placeholder `your.plugin`, the
> UI is intentionally generic, and every place that normally needs your own code is marked with
> a `TODO` comment.

The template builds, packs and installs without changes, so you can verify the toolchain first
and then replace the placeholders.

## 1. Copy and rename

1. Copy the folder to a new name, for example `MyPlugin`.
2. Rename `PluginTemplate.csproj` and set `AssemblyName` / `RootNamespace` if you want a
   different assembly name.
3. Rename the `PluginTemplate` namespace and `PluginTemplateExtension` / `TemplateWorker`
   classes in `PluginTemplateExtension.cs`.
4. Update `extension.json`:
   - `id`: your lowercase id, for example `me.myplugin` (letters, digits, `.` and `-`),
   - `name`, `description`, `author`, `version`,
   - `entryAssembly`: the DLL file name, for example `MyPlugin.dll`,
   - `entryType`: the full type name, for example `MyPlugin.MyPluginExtension`,
   - command ids and setting keys (and keep them in sync with the code),
   - `titleKey` values must exist in every locale file you ship.

The folder name does not have to match the assembly name, but `entryAssembly` must match the
DLL that is packaged.

## 2. What each part does

`PluginTemplateExtension.cs` is grouped into marked sections:

| Section | Purpose | Keep? |
| --- | --- | --- |
| Page | Main native Avalonia view opened by the command / menu / search | almost always |
| Instance tab | Per-instance view; receives `view.InstanceId` | optional |
| Command | Registered handler for the manifest command(s) | usually |
| Instance action | Adds an entry to the instance right-click menu | optional |
| Search provider | Adds results to the global command palette | optional |
| Event subscription | Listens to launcher events such as instance changes | optional |
| Worker | Separate process for long-running or risky work | optional |

Every `TODO` comment says what to replace. The important ones:

- `PageId` and all `ExtensionAction`/`ExtensionView` ids must start with your extension id plus
  a dot. The host validates this.
- `context.Settings.Get("greeting", ...)` reads a setting declared in `extension.json`.
  Add or remove settings in both places.
- `context.Text.Get("key")` reads the extension locale; add the matching key to every
  `Locales/<locale>.json` file.
- `context.Launcher.Notify(...)` shows a launcher notification; replace it with your action.
- `DisposeAsync` is where you release anything not owned by the host (windows, workers,
  subscriptions you created yourself).

`extension.json` cannot contain comments, so the fields are documented here and by the
`TODO`/`REPLACE ME` markers in the file.

## 3. Build

The project references the public extension SDK from a sibling PCL-ME checkout:

```text
..\PCL-ME\src\PCL.Extensions.Avalonia\PCL.Extensions.Avalonia.csproj
```

Adjust the `ProjectReference` in `PluginTemplate.csproj` if your checkout is elsewhere.

```powershell
dotnet build .\PluginTemplate.csproj -c Release
```

## 4. Pack and validate

```powershell
dotnet publish .\PluginTemplate.csproj -c Release -o .\artifacts\publish

dotnet run --project ..\PCL-ME\tools\PCL.Tools.Extensions -- pack `
  .\artifacts\publish .\artifacts\MyPlugin.pclext

dotnet run --project ..\PCL-ME\tools\PCL.Tools.Extensions -- validate `
  .\artifacts\MyPlugin.pclext
```

Rename the archive to your plugin name. `validate` checks the manifest, files and locale keys;
activation is checked when the launcher loads the extension.

## 5. Install

1. PCL-ME → **Settings → Extensions → Install**.
2. Select the `.pclext` archive.
3. Enable the extension in the manager.
4. Open the page from the **Extensions** menu, the global search
   (Ctrl+Shift+P / Cmd+Shift+P), or the command declared in `extension.json`.

A new external extension starts disabled. Updates that replace a loaded assembly take effect
after a restart.

## 6. Checklist before publishing

- [ ] Extension id, name, author and version changed in `extension.json`.
- [ ] `entryAssembly` and `entryType` match the built DLL and class.
- [ ] All UI contribution ids start with `<your id>.`.
- [ ] Commands/settings declared in the manifest are registered/read in code with the same keys.
- [ ] Every `titleKey` exists in every shipped locale.
- [ ] Unused optional sections removed from code and manifest.
- [ ] `DisposeAsync` releases your own resources.
- [ ] `pack` + `validate` succeed.
- [ ] Tested from a clean install (disable/enable, restart after update).

## 7. Rules of thumb

- Reference only the public SDK (`PCL.Extensions.Avalonia` / `PCL.Extensions.Contracts`). Never
  reference `PCL.Frontend.Avalonia` or `PCL.Core.Backend`; the loader rejects those.
- Use native Avalonia controls; the template uses `PclCard`, `PclButton` and `PclSearchBox` from
  `PCL.UI`, but plain Avalonia controls and XAML are fine too.
- Keep extension UI ids and settings keys namespaced. Do not rely on launcher internals or
  visual-tree structure.
- The extension page is a workspace overlay; use its back button to return to the launcher.
- A worker's stdout is reserved for the host protocol; log diagnostics to stderr.

## Files

```text
PluginTemplate.csproj          project + SDK reference
PluginTemplateExtension.cs     annotated IExtension entry point + worker sample
extension.json                 manifest: id, command, setting, entry type
Locales/en-US.json             English text
Locales/zh-Hans.json           Chinese text
README.md                      this guide
```