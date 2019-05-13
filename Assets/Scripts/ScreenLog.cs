using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLog : MonoBehaviour
{
    public static ScreenLog Instance;
    public Text text;
    public StringBuilder stringBuilder = new StringBuilder(100);
    void Start()
    {
        Instance = this;
    }

    public static void Log(string text)
    {
        if (Instance != null)
        {
            Instance.stringBuilder.Append(text + "\n");
            Instance.text.text = Instance.stringBuilder.ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
