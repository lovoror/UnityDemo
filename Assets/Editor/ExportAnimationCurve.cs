using UnityEngine;
using UnityEditor;

public class ExportAnimationCurve
{
    public static CurveClipData Execute(GameObject obj)
    {
        var _animations = obj.GetComponentsInChildren<Animation>();
        var _anim = _animations[0];
        var curveBindings = AnimationUtility.GetCurveBindings(_anim.clip);

        var curveClipData = new CurveClipData(curveBindings.Length);
        for(int i = 0; i < curveBindings.Length; i++)
        {
            var curveBinding = curveBindings[i];
            var curve = AnimationUtility.GetEditorCurve(_anim.clip, curveBinding);
            var curveData = curveClipData.m_curveDatas[i];
            curveData.m_animationCurve = curve;
            curveData.m_path = curveBinding.path;
            curveData.m_propertyName = curveBinding.propertyName;
            curveData.m_type = curveBinding.type;
        }
        foreach(var _animation in _animations)
        {
            RemoveAnimationComponent(_animation);
        }
        return curveClipData;
    }
    private static void RemoveAnimationComponent(Animation _animation)
    {
        var components = _animation.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].name == "Animation")
            {
                var serializedObject = new SerializedObject(_animation.gameObject);
                var prop = serializedObject.FindProperty("m_Component");
                prop.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}