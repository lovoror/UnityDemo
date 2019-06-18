using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
