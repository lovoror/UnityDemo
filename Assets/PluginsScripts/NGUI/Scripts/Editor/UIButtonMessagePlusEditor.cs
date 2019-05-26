//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIButtonPlusMessage))]
public class UIButtonMessagePlusEditor : Editor
{
	public override void OnInspectorGUI ()
	{
        EditorGUILayout.HelpBox("这个控件仅仅是用来处理技能按钮的长按操作\n其他的正常响应需求，请使用Event Trigger来处理", MessageType.Warning);
		base.OnInspectorGUI();
	}
}
