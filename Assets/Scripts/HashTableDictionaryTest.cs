using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

public class HashTableDictionaryTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IntMethod();
        StringMethod();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    static int count = 1000000;
    static void IntMethod()
    {
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        Hashtable hashtable = new Hashtable();
        for (int i = 0; i < count; i++)
        {
            dictionary.Add(i,i);
            hashtable.Add(i,i);
        }
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
        {
            int value = dictionary[i];
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
        stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
        {
            object value = hashtable[i];
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);

    }

    static void StringMethod()
    {
        Dictionary<string,string> dictionary = new Dictionary<string, string>();
        Hashtable hashtable = new Hashtable();
        for (int i = 0; i < count; i++)
        {
            dictionary.Add(i.ToString(),"aaa");
            hashtable.Add(i.ToString(), "bbb");

        }
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
        {
            string vale = dictionary[i.ToString()];
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
        stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
        {
            object value = hashtable[i.ToString()];
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
    }
}
