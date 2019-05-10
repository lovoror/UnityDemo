using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Sample : MonoBehaviour
{
    // 引入声明
    [DllImport("__Internal")]
    static extern void outputAppendString(string str1, string str2);
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IPHONE
        outputAppendString("Hello", "World");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
