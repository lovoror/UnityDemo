using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAsset()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        var outputPath = Application.streamingAssetsPath;
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        string[] buildPaths = { "Prefabs", "Texture", "Models" };
        foreach(var buildPath in buildPaths)
        {
            if (!Directory.Exists("Assets\\"+buildPath))
            {
                Debug.LogError("no found build path");
                return;
            }
            var dicInfo = new DirectoryInfo("Assets\\" + buildPath);
            var files = dicInfo.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension == ".meta")
                {
                    continue;
                }
                var index = file.FullName.IndexOf("Assets");
                var path = file.FullName.Substring(index);
                var importer = AssetImporter.GetAtPath(path);
                if (importer != null)
                {
                    var end = path.LastIndexOf(".");
                    var name = path.Substring(0, end);
                    importer.assetBundleName = name;
                    importer.assetBundleVariant = "bytes";
                }
                else
                {
                    Debug.LogErrorFormat("asset {0} is null", path);
                }
            }
        }
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
    }
    public void FindAsset()
    {
        // 查找根目录下prefab类型
        var assetFiles = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" });
        for (int i = 0; i < assetFiles.Length; i++)
        {
            AssetDatabase.GUIDToAssetPath(assetFiles[i]);
        }
    }
    // 获取文件的MD5码
    public static string GetFileMD5(string path)
    {
        FileStream file = new FileStream(path, FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(file);
        file.Close();

        StringBuilder sb = new StringBuilder();
        foreach(var val in retVal)
        {
            sb.Append(val.ToString("x2"));
        }
        return sb.ToString();
    }
}
