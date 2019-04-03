﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class TextEditor
{
    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool Step1(int instanceID, int line)
    {
        return false;
    }
    [UnityEditor.Callbacks.OnOpenAsset(2)]
    public static bool Step2(int instanceID, int line)
    {
        string strFilePath = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        string strFileName = System.IO.Directory.GetParent(Application.dataPath) + "/" + strFilePath;

        if (strFileName.EndsWith(".shader"))
        {
            string strSublimeTextPath = Environment.GetEnvironmentVariable("SublimeText_Path");
            if (strSublimeTextPath != null && strSublimeTextPath.Length > 0)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = strSublimeTextPath + (strSublimeTextPath.EndsWith("/") ? "" : "/") + "sublime_text.exe";
                startInfo.Arguments = "\"" + strFileName + "\"";
                process.StartInfo = startInfo;
                process.Start();
                return true;
            }
            else
            {
                Debug.LogError("Not Found Enviroment Variable 'SublimeText_Path'.");
                return false;
            }
        }
        return false;
    }
}
