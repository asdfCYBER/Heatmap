from pathlib import Path
from shutil import copy2, make_archive, rmtree


PATH_ASSETBUNDLES = Path("../../../../Documents/Heatmap UI/Heatmap UI/Assets/AssetBundles")
PATH_BINARIES = Path("../Heatmap/bin/Debug")
PATH_PACKAGE = Path("Heatmap")

# Make a temporary directory, all other files will be copied to this
# directory so that the directory can be zipped in its entirety by shutil
PATH_PACKAGE.mkdir()

# Copy files to the temporary directory
copy2(PATH_ASSETBUNDLES / "windows/heatmapuiassets", PATH_PACKAGE)
copy2(PATH_BINARIES / "Heatmap.dll", PATH_PACKAGE)
copy2(PATH_BINARIES / "Heatmap.Unity.dll", PATH_PACKAGE)
copy2(PATH_BINARIES / "0Harmony.dll", PATH_PACKAGE)
copy2(Path("license_distribute.txt"), PATH_PACKAGE)
(PATH_PACKAGE / "license_distribute.txt").rename(PATH_PACKAGE / "license.txt")

# Package for windows
make_archive("Heatmap_windows", "zip", Path(), PATH_PACKAGE)

# Replace heatmapuiassets with the one built for linux, and package
Path(PATH_PACKAGE, "heatmapuiassets").unlink()
copy2(PATH_ASSETBUNDLES / "linux/heatmapuiassets", PATH_PACKAGE)
make_archive("Heatmap_linux", "zip", Path(), PATH_PACKAGE)

# Replace heatmapuiassets with the one built for osx, and package
Path(PATH_PACKAGE, "heatmapuiassets").unlink()
copy2(PATH_ASSETBUNDLES / "linux/heatmapuiassets", PATH_PACKAGE)
make_archive("Heatmap_osx", "zip", Path(), PATH_PACKAGE)

# Remove Heatmap folder and subfolders
rmtree(PATH_PACKAGE)
