using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class UIScene : MonoBehaviour
{
    public static string[] sceneNames = { "Game", "Earth" };
    public GameObject m_template;
    public void SetName(GameObject obj)
    {
        obj.GetComponentInChildren<Text>().text = sceneNames[0];
    }
    // Start is called before the first frame update
    void Start()
    {
        var obj = m_template;
        for(int i= 0; i < sceneNames.Length; i++)
        {
            if (i != 0)
            {
                obj = Instantiate(m_template) as GameObject;
                obj.transform.parent = transform;
                obj.transform.localPosition = new Vector3(0, -20 * i, 0);
            }
            obj.GetComponentInChildren<Text>().text = sceneNames[i];
            //obj.GetComponent<Button>().onClick = OnClick;
        }
    }
    void OnClick()
    {

    }
}

#if UNITY_EDITOR
public static class GameViewUitls
{
    static object gameViewSizeGroupAndroid;
    static MethodInfo getGroup;
    public static string displayText;
    public static void Init()
    {
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizeGroupAndroid = getGroup.Invoke(instanceProp.GetValue(singleType), new object[] { (int)GameViewSizeGroupType.Android });
        var getDisplayTexts = gameViewSizeGroupAndroid.GetType().GetMethod("GetDisplayTexts");
        var displayTexts = getDisplayTexts.Invoke(gameViewSizeGroupAndroid, null) as string[];
        displayText = displayTexts[GetIndex()];
    }
    static int GetIndex()
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        return (int)selectedSizeIndexProp.GetValue(gvWnd);
    }
}
#endif