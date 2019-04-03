using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class LoadAssetBundle : MonoBehaviour
{
    public Transform m_Parent;
    public Text m_Text;
    static string m_Path;
    private StringBuilder m_StringBuilder = new StringBuilder(100);
    public void Awake()
    {
        m_Path = Application.dataPath + "/../output/a";
        if (m_Text != null)
        {
            m_Text.text = string.Format("dataPath = {0}\nstreamingAssetsPath = {1}\npersistentDataPath = {2}", Application.dataPath, Application.streamingAssetsPath, Application.persistentDataPath);
        }
    }
    public void Update()
    {
        if (m_Text != null)
        {
            m_StringBuilder.Clear();
            m_StringBuilder.Append("maxUsedMemory:");
            m_StringBuilder.Append(Profiler.maxUsedMemory.ToString());
            m_StringBuilder.Append("\nGetTotalAllocatedMemoryLong:");
            m_StringBuilder.Append(Profiler.GetTotalAllocatedMemoryLong());
            m_Text.text = m_StringBuilder.ToString();
        }
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
