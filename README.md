# Beat Saber Modding Tools
A set of templates for creating Beat Saber plugins.
**A wiki has been started [here](https://github.com/Zingabopp/BeatSaberModdingTools/wiki/1.-Home) with more details on using the extension**

**All Templates**
* Have the option of creating directory junctions to important folders in your Beat Saber directory (Managed/Libs/Plugins).
  * Drag your Beat Saber folder into "CreateJunctions.bat" inside your project directory.
  * Alternatively, specify the full path to your Beat Saber folder with the BeatSaberDir property inside a csproj.user file.
* Check AssemblyVersion.cs against the plugin version specified in manifest.json and issue a build error message when they don't match.
* Automatically zip Release builds using the file name format <AssemblyName>-<AssemblyVersion>-bs<BeatSaberGameVersion>-<GithubCommitHash>.zip.
  * AssemblyName, AssemblyVersion, and BeatSaberGameVersion are read from the manifest.json.
* Copies the plugin dll to BeatSaberDir\Plugins.

# Prerequisites
* Templates can be manually installed for Visual Studio 2017 and 2019.
* The optional BeatSaberModdingTools VSIX requires Visual Studio 2019.
* All templates require at least .Net Framework 4.7.2, you can get the Developer Pack from [Here](https://dotnet.microsoft.com/download/visual-studio-sdks).
* To develop mods you have to have the game files and BSIPA.

# How To Use
* Templates can be installed in one of two ways:
  * If you have Visual Studio 2019, you can download BeatSaberTemplateInstaller.vsix from the [Releases](https://github.com/Zingabopp/BeatSaberTemplates/releases) page and run it.
    * **Important:** *If you manually added the templates to your Visual Studio Templates folder, they may override the installer's templates.*
    * You can download and run newer versions of the VSIX installer, they will replace the old version.
  * Copy the template zip (or folder) into "\<UserFolder>\Documents\Visual Studio <2019/2017>\Templates\ProjectTemplates\C#"
* In Visual Studio, create a new project using the template.
* Set your Beat Saber game folder path in one of two ways:
  * If you are using the VSIX:
    1. Right-click your project.
    2. Mouse over "Beat Saber Modding Tools"
    3. Click "Set Beat Saber Directory..."
  * Open your project folder and drag your Beat Saber game folder onto CreateJunctions.bat.
  * Use a csproj.user file:
    1. Copy [ProjectName.csproj.user](https://github.com/Zingabopp/BeatSaberTemplates/blob/master/ProjectName.csproj.user) to your project folder and rename it so ProjectName matches the name on your .csproj file.
    2. Open the csproj.user file and put the path to your Beat Saber folder inside the \<BeatSaberDir> tag.

# Beat Saber Modding Tools VSIX Features
* Settings to make creating mods more convenient
  * Set your Beat Saber install directory.
  * Automatically set the BeatSaberDir property in opened projects if available.
  * Located in the Menu Bar > Extensions > Beat Saber Modding Tools > Settings...
* Beat Saber Reference Manager
  * Allows you to add/remove references from your Managed/Libs/Plugins folders.
  * References automatically use BeatSaberDir for their HintPath.
  * Open by right-clicking a BSIPA project's References > Beat Saber Reference Manager...
* Add Reference Paths
  * Allows you to tell Visual Studio where to find the Beat Saber references for projects that don't use the BeatSaberDir property.
  * Located by right-clicking a BSIPA project > Beat Saber Modding Tools > Add Reference Paths
  
# Available Templates
* **BSIPA Full Template:** This template demonstrates how to create a basic plugin and use CustomUI. It also has examples for using Harmony and ReflectionUtil.
* **BSIPA Core Template:** This template creates a bare plugin.
* **BSIPA Disableable Template:** This template creates a plugin that can be enable/disabled while the game is running.
