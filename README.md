# Beat Saber Modding Tools
**Starting with version 0.9.4, all templates have been moved to their own extension. [UnityModdingTools.Templates.BeatSaber](https://marketplace.visualstudio.com/items?itemName=Zingabopp.BeatSaberModTemplates) can be downloaded from the Visual Studio Marketplace (website or inside Visual Studio).**

Convenience IDE features for creating Beat Saber mods. The extension (VSIX) only works for Visual Studio 2019 and 2022. A Rider port with dotnet templates is being worked on [Here](https://github.com/Fernthedev/BSMT-Rider).
**A wiki has been started [here](https://github.com/Zingabopp/BeatSaberModdingTools/wiki) with more details on using the extension**

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

# Prerequisites
* The optional BeatSaberModdingTools VSIX requires Visual Studio 2019 or 2022.
* You must have the `.NET desktop development` workload installed for Visual Studio.
* All templates require at least .Net Framework 4.7.2, you can get the Developer Pack from [Here](https://dotnet.microsoft.com/download/visual-studio-sdks).
* To develop mods you have to have the game files and BeatSaber-IPA.
