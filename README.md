# Heatmap
[![CodeFactor grade](https://img.shields.io/codefactor/grade/github/asdfcyber/heatmap)](https://www.codefactor.io/repository/github/asdfcyber/heatmap) [![Latest release](https://img.shields.io/github/v/release/asdfcyber/heatmap)](https://github.com/asdfcyber/heatmap/releases/latest) ![Platforms](https://img.shields.io/badge/platform-windows%20%7C%20macos%20%7C%20linux-blue) <br/><br/>


## Description
A mod for [Rail Route](https://railroute.bitrich.info/) which colors tracks based on measured busyness. Using Heatmap will give you insight into the busyness of your rail network, which means you can quickly see where you can fit more trains and which intersections need to be redesigned. As of version 1.0.0, Heatmap includes all Perceptually Uniform Sequential colormaps from Matplotlib, a simple green-red colormap and a simple blue-red colormap.<br/><br/>


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
When playing a timetable or endless map with the mod installed, you will see a new button with a fire icon in the toolbar on the left. If you click it, the tracks will be colored based on their busyness and a couple of settings. You can change the settings by right-clicking the button. These settings are currently implemented:

- **Mode:** which values will be used for the colors and tooltips. As of version 1.2.0, possible modes are *time spent* (the amount of time trains have been in the node within the measuring period), *max. speed* (the highest velocity allowed in the node), *avg. speed* (the average velocity of trains in the node), *node length*, *visits* (how often trains have been in the node within the measuring period) and *total visits* (how often trains have been in the node since the node was created).
- **Colormap:** the colormap used to color the tracks. You can see what the colormaps look like on the [Matplotlib website](https://matplotlib.org/stable/tutorials/colors/colormaps.html#sequential) (except for the green-red and blue-red colormaps, but those should be self-explanatory).
- **Measuring period:** the amount of minutes that Heatmap 'looks into the past' to determine the busyness. The default is 30. Note that it takes the same amount of time to gather data, meaning that it takes 30 in-game minutes for an occupied track to reach 100% busyness for the default value.
- **Minimum value:** the value that will be given the lowest value color (color depends on the selected colormap).
- **Maximum value:** the value that will be given the highest value color (color depends on the selected colormap). Cannot be lower than the minimum value.
- **Delete data after:** the amount of minutes that gathered data will be retained. Default is 30 minutes, and it cannot be lower than the measuring period.


When Heatmap is enabled, hovering over tracks will display a tooltip with the values used for generating the color of that track.<br/><br/>



## Reporting issues
If you find a bug, please start a new GitHub issue or message me on the Rail Route discord. Don't forget to send your `Player.log` file! You can find it one folder up from the mod folder. A screenshot or savefile might be helpful too. If you think you found a bug in Rail Route and you're using Heatmap, first remove/disable the mod to make sure it isn't actually a Heatmap issue.
