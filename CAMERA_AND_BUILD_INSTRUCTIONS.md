Purpose

This file documents how to verify the ThirdPersonCamera (C#) is setup, build the C# assemblies, open the project in Godot, bake the NavigationMesh, and verify the InputMap entries we persisted into `project.godot`.

Quick checklist

- [ ] Build C# assemblies (Mono) so Godot can load `ThirdPersonCamera.cs`.
- [ ] Open Godot and reload the project (restart the editor to clear caches).
- [ ] Open `scenes/main/main_scene.tscn` and verify the scene loads without errors.
- [ ] Bake the NavigationMesh for `NavigationRegion3D` in the editor.
- [ ] Verify InputMap actions exist in Project Settings -> Input Map or by checking `project.godot`.
- [ ] If you see old errors, clear editor editstate cache (commands below) and retry.

1) Build C# assemblies (recommended: Visual Studio / Rider)

- Preferred: open `Diablo.sln` in Visual Studio or Rider and run Build -> Build Solution.

- Quick PowerShell (dotnet) example (works if dotnet SDK and solution are configured for Godot Mono builds):

```powershell
cd 'D:\Project\Godot\diablo'
# Build the solution in Debug (adjust configuration if needed)
dotnet build "Diablo.sln" -c Debug
```

2) Start Godot editor and open the project

- Replace the exe path below with your Godot 4.6.1 Mono executable path:

```powershell
& 'C:\Program Files\Godot\Godot 4.6.1\Godot.exe' -path 'D:\Project\Godot\diablo'
```

- Restart Godot if it was already open (to avoid stale cached state).

3) Open and validate the main scene

- In the Godot editor, open `scenes/main/main_scene.tscn`.
- Confirm there is NO error like "Invalid scene: root node DirectionalLight3D cannot specify a parent node.".
- Confirm the `Camera3D` node has the `TargetPath` property set to `../Player` (Inspector -> Camera3D -> TargetPath).

4) Bake NavigationMesh (editor-only step)

- Select `NavigationRegion3D` node in the 3D scene tree.
- Use the 3D viewport top toolbar (or Inspector) and click Bake (NavigationMesh) to generate polygons.
- Save the scene.

5) Verify InputMap actions

- In Godot: Project -> Project Settings -> Input Map, confirm actions (e.g., `cast_spell`, `attack`, `move_forward`, `camera_rotate` etc.) are present and bound.
- Or quickly check `project.godot` from PowerShell to find a specific action (example: `cast_spell`):

```powershell
Select-String -Path 'D:\Project\Godot\diablo\project.godot' -Pattern 'cast_spell' -Context 0,2
```

6) If Godot still shows the "Invalid scene" or cache-related errors

- Close Godot then run these PowerShell commands from the repo root to remove editor editstate and filesystem cache (I backed up the files earlier):

```powershell
# remove main_scene editor cache and folding states (safe to remove; backups exist)
Remove-Item -Path .godot\editor\main_scene.tscn-editstate-* -Force -ErrorAction SilentlyContinue
Remove-Item -Path .godot\editor\main_scene.tscn-folding-* -Force -ErrorAction SilentlyContinue

# optional: clear other editor caches if necessary
Remove-Item -Path .godot\editor\filesystem_cache* -Force -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path .godot\editor\script_editor_cache.cfg -Force -ErrorAction SilentlyContinue
```

Then reopen Godot and re-open the scene.

7) Reverting changes

- I backed up modified files before editing. To restore the previous `main_scene.tscn` you can run:

```powershell
Copy-Item 'D:\Project\Godot\diablo\scenes\main\main_scene.tscn.bak' -Destination 'D:\Project\Godot\diablo\scenes\main\main_scene.tscn' -Force
```

- To restore editor backups (if needed):

```powershell
Copy-Item '.godot\editor\main_scene.tscn-editstate-b52e533425a7309d45deb665fa5bfd25.cfg.bak' -Destination '.godot\editor\main_scene.tscn-editstate-b52e533425a7309d45deb665fa5bfd25.cfg' -Force
Copy-Item '.godot\editor\main_scene.tscn-folding-b52e533425a7309d45deb665fa5bfd25.cfg.bak' -Destination '.godot\editor\main_scene.tscn-folding-b52e533425a7309d45deb665fa5bfd25.cfg' -Force
```

8) Notes & troubleshooting

- If the C# script fails to load in Godot, make sure assemblies were built and Godot's Mono version matches your SDK.
- If you still see repeated errors like "Dictionary::operator[] used when there was no value for the given key 'events'", confirm `project.godot` contains `actions/<name>={events=[...], deadzone=...}` entries. I updated `project.godot` to follow that structure.

Contact me the editor output (copy any console error text) if any of these steps still produce errors and I'll continue debugging immediately.

