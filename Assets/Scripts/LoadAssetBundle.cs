using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System.Collections;

public class LoadAssetBundle : MonoBehaviour
{
    public Transform m_Parent;
    public Text AsyncTime;
    public Text SyncTime;
    static string m_Path;
    private StringBuilder m_StringBuilder = new StringBuilder(100);
    public void Awake()
    {
        m_Path = Application.dataPath + "/../output/a";
        //if (m_Text != null)
        //{
        //    m_Text.text = string.Format("dataPath = {0}\nstreamingAssetsPath = {1}\npersistentDataPath = {2}", Application.dataPath, Application.streamingAssetsPath, Application.persistentDataPath);
        //}
    }
    public void Update()
    {
        //if (m_Text != null)
        //{
        //    m_StringBuilder.Clear();
        //    m_StringBuilder.Append("maxUsedMemory:");
        //    m_StringBuilder.Append(Profiler.maxUsedMemory.ToString());
        //    m_StringBuilder.Append("\nGetTotalAllocatedMemoryLong:");
        //    m_StringBuilder.Append(Profiler.GetTotalAllocatedMemoryLong());
        //    m_Text.text = m_StringBuilder.ToString();
        //}
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
    List<AssetBundle> list = new List<AssetBundle>();
    public void LoadSync()
    {
        float start = Time.realtimeSinceStartup;
        for (int i = 0; i < 16; i++)
        {
            string filename = Application.streamingAssetsPath + "/assets/texture/" + "3 - 副本 (" + i.ToString() + ").bytes";
            AssetBundle a = AssetBundle.LoadFromFile(filename);
            list.Add(a);
            Texture2D t = a.LoadAsset<Texture2D>(a.GetAllAssetNames()[0]);
        }
        float end = Time.realtimeSinceStartup;
        SyncTime.text = "同步加载 Spend Time" + (end - start).ToString() + "s";
        foreach(var l in list)
        {
            l.Unload(true);
        }
        list.Clear();
    }
    public void LoadAsync()
    {
        StartCoroutine(LoadAsyncWrap());
    }
    bool[] isloadover = new bool[16];
    IEnumerator LoadAsyncWrap()
    {
        float a = Time.realtimeSinceStartup;
        for (int i = 0; i < isloadover.Length; i++)
        {
            isloadover[i] = false;
        }
        for (int i = 0; i < 16;i++)
        {
            string filename = Application.streamingAssetsPath + "/assets/texture/" + "3 - 副本 (" + i.ToString() + ").bytes";
            StartCoroutine(LoadOne(filename, i));
        }
        while (!Isallloadover())
        {
            yield return 1;
        }
        float b = Time.realtimeSinceStartup;
        AsyncTime.text = "异步加载时间 Spend Time" + (b - a).ToString() + "s";
    }
    bool Isallloadover()
    {
        for (int i = 0; i < isloadover.Length; i++)
        {
            if (!isloadover[i])
            {
                return false;
            }
        }
        return true;
    }
    IEnumerator LoadOne(string filename, int index)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(filename);
        yield return request;
        Texture2D t = request.assetBundle.LoadAsset<Texture2D>(request.assetBundle.GetAllAssetNames()[0]);
        isloadover[index] = true;
    }
}
