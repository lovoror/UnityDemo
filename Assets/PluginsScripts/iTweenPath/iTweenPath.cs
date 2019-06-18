//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using System.Collections.Generic;

public class iTweenPath : MonoBehaviour
{
	public Color pathColor = Color.cyan;
	public List<Vector3> nodes = new List<Vector3>(){Vector3.zero, Vector3.zero};
    public List<int> nodeTimesKey = new List<int>();
    public List<float> nodeTimesVal = new List<float>();
	public int nodeCount;
	public static Dictionary<string, iTweenPath> paths = new Dictionary<string, iTweenPath>();
	public bool initialized = false;
    public bool flyPath = true;
    public string pathName
    {
        get
        {
            return name;
        }
    }
	
	void OnEnable()
    {
        if (!paths.ContainsKey(pathName.ToLower()))
        {
            paths.Add(pathName.ToLower(), this);
        }
	}
	
	void OnDrawGizmosSelected()
    {
		if(enabled) 
        { 
			if(nodes.Count > 0)
            {
                if (flyPath)
                {
                    iTween.DrawPath(nodes.ToArray(), pathColor);
                }
                else
                {
                    iTween.DrawLine(nodes.ToArray(), pathColor);
                }
			}
		} 
	}

    public static iTweenPath Get(string requestedName)
    {
        requestedName = requestedName.ToLower();
		if(paths.ContainsKey(requestedName))
        {
            return paths[requestedName];
        }
        return null;
    }

	public static Vector3[] GetPath(string requestedName)
    {
		requestedName = requestedName.ToLower();
		if(paths.ContainsKey(requestedName))
        {
			return paths[requestedName].nodes.ToArray();
		}
            
        Debug.LogError("没有找到线路 "+requestedName);
        return null;
	}

}

