using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(UICardMove))]
public class UICardMoveInspector : Editor
{
    private GUIContent m_guiContent;
    private UICardMove m_uiCardMove;
    private SerializedProperty m_curveClipData;
    private SerializedProperty m_autoAdjust;
    private void OnEnable()
    {
        m_guiContent = new GUIContent("导出Animation", "将子节点的Animation中的Curve导出，用于处理屏幕自适应");
        m_uiCardMove = target as UICardMove;
        m_curveClipData = serializedObject.FindProperty("m_curveClipData");
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button(m_guiContent))
        {
            m_uiCardMove.m_curveClipData = ExportAnimationCurve.Execute(m_uiCardMove.gameObject);
        }
        EditorGUILayout.PropertyField(m_curveClipData, true);
    }
}