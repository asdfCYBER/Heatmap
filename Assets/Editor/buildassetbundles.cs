using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        EnsureDirectory(assetBundleDirectory);

        #region windows
        string windowsPath = assetBundleDirectory + "/windows";
        EnsureDirectory(windowsPath);

        BuildPipeline.BuildAssetBundles(
            windowsPath,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows64);
        #endregion

        #region linux
        string linuxPath = Path.Combine(assetBundleDirectory, "linux");
        EnsureDirectory(linuxPath);

        BuildPipeline.BuildAssetBundles(
            linuxPath,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneLinux64);
        #endregion

        #region osx 
        string osxPath = Path.Combine(assetBundleDirectory, "osx");
        EnsureDirectory(osxPath);

        BuildPipeline.BuildAssetBundles(
            osxPath,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneOSX);
        #endregion
    }

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}