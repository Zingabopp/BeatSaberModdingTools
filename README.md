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


# How To Use
* Copy the template zip (or folder) into "\<UserFolder>\Documents\Visual Studio <2019/2017>\Templates\ProjectTemplates\C#"
* In Visual Studio, create a new project using the template.
* Set your Beat Saber game folder path in one of two ways:
  * Open your project folder and drag your Beat Saber game folder onto CreateJunctions.bat.
  * Use a csproj.user file:
    1. Copy [ProjectName.csproj.user](https://github.com/Zingabopp/BeatSaberTemplates/blob/master/ProjectName.csproj.user) to your project folder and rename so ProjectName matches the name on your .csproj file.
    2. Open the csproj.user file and put the path to your Beat Saber folder inside the \<BeatSaberFolder> tag.

# BSIPA Full Template
* This template demonstrates how to create a basic mod and how to use CustomUI.
