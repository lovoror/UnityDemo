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
        var path = Application.dataPath + "/../output";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var buildPath = "Assets/Prefabs";
        if (!Directory.Exists(buildPath))
        {
            Debug.LogError("no found build path");
            return;
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
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
