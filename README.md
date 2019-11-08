# BeatSaberTemplates
A set of templates for creating Beat Saber mods.

**All Templates**
* Have the option of creating directory junctions to important folders in your Beat Saber directory (Managed/Libs/Plugins).
  * Drag your Beat Saber folder into "CreateJunctions.bat" inside your project directory.
  * Alternatively, specify the full path to your Beat Saber folder with the BeatSaberFolder property inside a csproj.user file.
* Check AssemblyVersion.cs against the mod version specified in manifest.json and issue a build error message when they don't match.
* Automatically zip Release builds using the file name format <AssemblyName>-<AssemblyVersion>-bs<BeatSaberGameVersion>-<GithubCommitHash>.zip.
  * AssemblyName, AssemblyVersion, and BeatSaberGameVersion are read from the manifest.json.
* Copies the mod dll to BeatSaberFolder\Plugins.

# BSIPA Full Template
* This template demonstrating how to create a basic mod and how to use CustomUI.
