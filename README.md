# Heatmap
[![CodeFactor grade](https://img.shields.io/codefactor/grade/github/asdfcyber/heatmap)](https://www.codefactor.io/repository/github/asdfcyber/heatmap) [![Latest release](https://img.shields.io/github/v/release/asdfcyber/heatmap)](https://github.com/asdfcyber/heatmap/releases/latest) ![Platforms](https://img.shields.io/badge/platform-windows%20%7C%20macos%20%7C%20linux-blue) <br/><br/>


## Description
A mod for [Rail Route](https://railroute.bitrich.info/) which colors tracks based on measured busyness or other metrics. Using Heatmap will give you insight into the busyness of your rail network, which means you can quickly see where you can fit more trains and which intersections need to be redesigned. You can choose one of the predefined colormaps such as the Perceptually Uniform Sequential colormaps from Matplotlib, or create your own.<br/><br/>


## How to install
Download the latest version for your platform, and unpack the zip in the right directory depending on your operating system:

**Windows:** `%USERPROFILE%\AppData\LocalLow\bitrich\Rail Route\mods`  
**Linux:** `$HOME/.config/unity3d/bitrich/Rail Route/mods`  
**macOS:** `~/Library/Application Support/Rail Route/mods`

If you did it right, you should have the following file structure:
```
mods
|
| - Heatmap
|   |
|   | - Heatmap.dll and other files
```

You can uninstall the mod by deleting the Heatmap folder and its contents. This won't break your game or corrupt savegames. If you want to temporarily disable Heatmap without deleting anything for whatever reason, you can change the file extension of `Heatmap.dll` or move it somewhere else.<br/><br/>


## How to use
When playing a timetable or endless map with the mod installed, you will see a new button with a fire icon in the interface menu. If you click it or use the shortcut `h`, the tracks will be colored based on their busyness and a couple of settings. You can change the settings by doubleclicking the button or pressing `h` twice. These settings are currently implemented:

- **Mode:** which values will be used for the colors and tooltips. Possible modes are *time spent* (the amount of time trains have been in the node within the measuring period), *max. speed* (the highest velocity allowed in the node), *avg. speed* (the average velocity of trains in the node), *node length*, *visits* (how often trains have been in the node within the measuring period) and *total visits* (how often trains have been in the node since the node was created).
- **Colormap:** the colormap used to color the tracks. You can see what the colormaps look like by pressing the configuration button next to it. You can also create or edit your own colormaps there.
- **Measuring period:** the amount of minutes that Heatmap 'looks into the past' to determine the busyness. The default is 30. Note that it takes the same amount of time to gather data, meaning that it takes 30 in-game minutes for an occupied track to reach 100% busyness for the default value.
- **Minimum value:** the value that will be given the lowest value color (color depends on the selected colormap).
- **Maximum value:** the value that will be given the highest value color (color depends on the selected colormap). Cannot be lower than the minimum value.
- **Delete data after:** the amount of minutes that gathered data will be retained. Default is 30 minutes, and it cannot be lower than the measuring period.


When Heatmap is enabled, hovering over tracks will display a tooltip with the value for the selected mode.<br/><br/>



## Reporting issues
If you find a bug, please start a new GitHub issue or message me on the Rail Route discord. Don't forget to send your `Player.log` file! You can find it one folder up from the mod folder. A screenshot or savefile might be helpful too. If you think you found a bug in Rail Route and you're using Heatmap, first remove/disable the mod to make sure it isn't actually a Heatmap issue.
