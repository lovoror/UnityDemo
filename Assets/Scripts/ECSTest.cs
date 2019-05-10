using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSTest : MonoBehaviour
{
    void OnFinish(AsyncOperation req)
    {
        AssetBundleCreateRequest abcreq = req as AssetBundleCreateRequest;
        var ab = abcreq.assetBundle;
        if (ab == null)
            return;
        var abreq = ab.LoadAssetAsync("test");
        abreq.completed += OnAssetBundleCompleted;
    }
    Object obj;
    void OnAssetBundleCompleted(AsyncOperation ao)
    {
        AssetBundleRequest abreq = ao as AssetBundleRequest;
        obj = abreq.asset;
    }
    // Start is called before the first frame update
    void Start()
    {
        var quest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/assets/models/test");
        if (quest == null)
        {
            Debug.LogError("LoadFromFileAsync failed");
            return;
        }
        quest.completed += OnFinish;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            AddShips(1);
        }
    }
    static float leftRound = 0;
    static float rightRound = 100;
    static float topBound = 100;
    void AddShips(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(leftRound, rightRound);
            float zVal = Random.Range(0f, 10f);
            Vector3 pos = new Vector3(xVal, 0f, zVal + topBound);
            Quaternion rot = Quaternion.Euler(0f, 180f, 0f);
            if (obj != null)
            {
                var o = Instantiate(obj, pos, rot);
            }
        }
    }
}
