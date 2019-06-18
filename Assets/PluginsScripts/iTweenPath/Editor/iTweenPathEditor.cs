//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(iTweenPath))]
public class iTweenPathEditor : Editor
{
	iTweenPath _target;
    int nodeCount;
	GUIStyle style = new GUIStyle();
	public static int count = 0;

    [MenuItem("Component/iTween/iTweenPath")]
    static void AddiTweenEvent()
    {
        if (Selection.activeGameObject != null)
        {
            Selection.activeGameObject.AddComponent(typeof(iTweenPath));
        }
    }

	void OnEnable(){
		//i like bold handle labels since I'm getting old:
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (iTweenPath)target;
        nodeCount = _target.nodeCount;
		//lock in a default path name:
		if(!_target.initialized){
			_target.initialized = true;
		}
	}

    Vector2 pos = Vector2.zero;
    Vector2 nodeTimeScrollPos = Vector2.zero;
    static int iTick;

	public override void OnInspectorGUI(){		

		//path color:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Color");
		_target.pathColor = EditorGUILayout.ColorField(_target.pathColor);
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Fly Path");
        _target.flyPath = EditorGUILayout.Toggle(_target.flyPath);
        EditorGUILayout.EndHorizontal();

		//exploration segment count control:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Node Count");

        nodeCount = Mathf.Clamp(EditorGUILayout.IntSlider(nodeCount, 0, 100), 2, 100);
        if (GUILayout.Button("OK"))
        {
            _target.nodeCount = nodeCount;
        }
		EditorGUILayout.EndHorizontal();

        pos = EditorGUILayout.BeginScrollView(pos);

		//add node?
        if (_target.nodeCount > _target.nodes.Count)
        {
            _target.nodes.Add(_target.nodes[_target.nodes.Count-1]);
		}
        else if (_target.nodeCount < _target.nodes.Count)
        {//remove node?
            _target.nodes.RemoveRange(_target.nodeCount - 1, _target.nodes.Count - _target.nodeCount);
		}

		//node display:
		EditorGUI.indentLevel = 4;
        for (int i = 0; i < _target.nodes.Count; i++)
        {
			_target.nodes[i] = EditorGUILayout.Vector3Field("Node " + (i+1), _target.nodes[i]);
		}

        EditorGUILayout.EndScrollView();

        EditorGUI.indentLevel = 0;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Node Time");
        if( GUILayout.Button("Add", GUILayout.Width(50)) )
        {
            int timeCount = _target.nodeTimesKey.Count;
            if (_target.nodeTimesKey.BinarySearch(timeCount) < 0)
            {
                _target.nodeTimesKey.Add(timeCount);
                _target.nodeTimesVal.Add(1f);
            }
            else
            {
                for( int i = 0; i < timeCount; ++i )
                {
                    if (_target.nodeTimesKey.BinarySearch(i) < 0 )
                    {
                        _target.nodeTimesKey.Add(i);
                        _target.nodeTimesVal.Add(1f);
                        break;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        string[] content = new string[nodeCount];
        for( int i = 0; i < nodeCount; ++i )
        {
            content[i] = i.ToString();
        }

        Dictionary<int, float> newNodeTimes = new Dictionary<int, float>();
        nodeTimeScrollPos = EditorGUILayout.BeginScrollView(nodeTimeScrollPos);
        int timeKeyCount = _target.nodeTimesKey.Count;
        for (int i = 0; i < timeKeyCount; ++i )
        {
            int oldKey = _target.nodeTimesKey[i];
            float oldVal = _target.nodeTimesVal[i];

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(22), GUILayout.Height(14)))
            {
                newNodeTimes.Remove(oldKey);
                continue;
            }
            int newKey = EditorGUILayout.Popup(oldKey, content, GUILayout.Width(50f), GUILayout.Height(16));
            float newVal = EditorGUILayout.FloatField("StopTime", oldVal);
            EditorGUILayout.EndHorizontal();

            newNodeTimes.Remove(oldKey);
            newNodeTimes.Remove(newKey);
            newNodeTimes.Add(newKey, newVal);
        }

        _target.nodeTimesKey.Clear();
        _target.nodeTimesVal.Clear();
        foreach(var item in newNodeTimes)
        {
            int key = item.Key;
            float val = item.Value;

            _target.nodeTimesKey.Add(key);
            _target.nodeTimesVal.Add(val);
        }
        EditorGUILayout.EndScrollView();


        if(GUI.changed)
        {
            EditorUtility.SetDirty(_target);			
        }
	}
	
	void OnSceneGUI(){
		if(_target.enabled) { // dkoontz
            if (_target.nodes.Count > 0)
            {
				//allow path adjustment undo:
                Undo.RecordObject(_target, "Adjust iTween Path");
				
				//path begin and end labels:
				Handles.Label(_target.nodes[0], "'" + _target.pathName + "' Begin", style);
                if( _target.nodes.Count > 2 )
                {
                    for( int i = 1; i < _target.nodes.Count-1; ++i )
                    {
                        Handles.Label(_target.nodes[i], "Point(" + i + ")");
                    }
                }
				Handles.Label(_target.nodes[_target.nodes.Count-1], "'" + _target.pathName + "' End", style);
				
				//node handle display:
                for (int i = 0; i < _target.nodes.Count; i++)
                {
					_target.nodes[i] = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
				}	
			}
		} // dkoontz
	}
}