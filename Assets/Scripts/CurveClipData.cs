using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CurveClipData
{
    [Serializable]
    public enum PropertyType
    {
        LocalPosition,
        Rotation_x,
        Scale,
    }
    [Serializable]
    public class CurveData
    {
        public AnimationCurve[] m_animationCurves;
        public string m_path;
        public PropertyType m_propertyType;
    }
    public CurveData[] m_curveDatas;

    public void Sample(GameObject obj, float time)
    {
        foreach(var curveData in m_curveDatas)
        {
            Transform temp = obj.transform;
            if (!string.IsNullOrEmpty(curveData.m_path))
            {
                temp = obj.transform.Find(curveData.m_path);
            }
            switch (curveData.m_propertyType)
            {
                case PropertyType.LocalPosition:
                    var vector3 = new Vector3
                    {
                        x = curveData.m_animationCurves[0].Evaluate(time),
                        y = curveData.m_animationCurves[1].Evaluate(time),
                        z = curveData.m_animationCurves[2].Evaluate(time)
                    };
                    temp.localPosition = vector3;
                    break;
            }
        }
    }
}
