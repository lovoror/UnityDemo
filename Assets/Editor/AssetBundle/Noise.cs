using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Noise
{
    [MenuItem("Tools/BuildWhiteNoise")]
    public static void BuildWhiteNoise()
    {
        var tex2d = new Texture2D(100, 100);
        var time = System.DateTime.Now.Ticks;
        System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/" + time + ".png", tex2d.GetRawTextureData());
        AssetDatabase.Refresh();
    }
}
