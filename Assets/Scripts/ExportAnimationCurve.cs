#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ExportAnimationCurve
{
    private static List<CurveClipData.CurveData> curveDatas = new List<CurveClipData.CurveData>();
    public static CurveClipData Execute(GameObject obj)
    {
        curveDatas.Clear();
        var _animations = obj.GetComponentsInChildren<Animation>();
        if (_animations == null || _animations.Length == 0)
            return null;
        var _anim = _animations[0];
        var curveBindings = AnimationUtility.GetCurveBindings(_anim.clip);

        var curveClipData = new CurveClipData();
        for(int i = 0; i < curveBindings.Length; i++)
        {
            var curveBinding = curveBindings[i];
            var curve = AnimationUtility.GetEditorCurve(_anim.clip, curveBinding);
            CurveClipData.CurveData curveData = null;
            switch (curveBinding.propertyName)
            {
                case "m_LocalPosition.x":
                    curveData = GetCurveByPath(curveBinding.path, CurveClipData.PropertyType.LocalPosition);
                    curveData.m_animationCurves[0] = curve;
                    break;
                case "m_LocalPosition.y":
                    curveData = GetCurveByPath(curveBinding.path, CurveClipData.PropertyType.LocalPosition);
                    curveData.m_animationCurves[1] = curve;
                    break;
                case "m_LocalPosition.z":
                    curveData = GetCurveByPath(curveBinding.path, CurveClipData.PropertyType.LocalPosition);
                    curveData.m_animationCurves[2] = curve;
                    break;
            }
        }
        foreach(var _animation in _animations)
        {
            RemoveAnimationComponent(_animation);
        }
        //var assetObj = PrefabUtility.GetCorrespondingObjectFromSource(obj);
        //if (assetObj)
        //{
        //    string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(assetObj);
        //    PrefabUtility.SaveAsPrefabAssetAndConnect(obj, assetPath, InteractionMode.AutomatedAction);
        //    PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        //    PrefabUtility.SaveAsPrefabAssetAndConnect(obj, assetPath, InteractionMode.AutomatedAction);
        //}
        AssetDatabase.SaveAssets();
        curveClipData.m_curveDatas = curveDatas.ToArray();
        return curveClipData;
    }
    public static CurveClipData.CurveData GetCurveByPath(string path, CurveClipData.PropertyType propertyType)
    {
        foreach(var curveData in curveDatas)
        {
            if (curveData.m_path == path)
            {
                return curveData;
            }
        }
        var _curveData = new CurveClipData.CurveData
        {
            m_propertyType = propertyType,
            m_path = path
        };
        switch (propertyType)
        {
            case CurveClipData.PropertyType.LocalPosition:
                _curveData.m_animationCurves = new AnimationCurve[3];
                break;
        }
        curveDatas.Add(_curveData);
        return _curveData;
    }
    private static void RemoveAnimationComponent(Animation _animation)
    {
        var components = _animation.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].GetType() == typeof(Animation))
            {
                var serializedObject = new SerializedObject(_animation.gameObject);
                var prop = serializedObject.FindProperty("m_Component");
                Object.DestroyImmediate(components[i]);
                prop.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}
#endif