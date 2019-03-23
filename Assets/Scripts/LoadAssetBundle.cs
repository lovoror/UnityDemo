using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundle : MonoBehaviour
{
    public Transform m_Parent;
    static string m_Path;
    public void Awake()
    {
        m_Path = Application.dataPath + "/../output/a";
    }
    static Dictionary<string, AssetBundle> m_AssetBundleDic = new Dictionary<string, AssetBundle>();
    public void OnClickLoadAssetBundle()
    {
        AssetBundle ab;
        m_AssetBundleDic.TryGetValue(m_Path, out ab);
        if (ab == null)
        {
            ab = AssetBundle.LoadFromFile(m_Path);
            if (ab == null)
            {
                Debug.LogError("assetbundle load failed");
                return;
            }
        }
        var obj = ab.LoadAsset<GameObject>("a");
        if (obj == null)
        {
            Debug.LogError("asset load failed");
            return;
        }
        var go = Instantiate(obj);
        if (go == null)
        {
            Debug.LogError("instantiate obj failed");
            return;
        }
        go.transform.parent = m_Parent;
    }
}
