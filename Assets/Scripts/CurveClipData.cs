using UnityEngine;
using System;

public class CurveClipData
{
    public class CurveData
    {
        public AnimationCurve m_animationCurve;
        public string m_path;
        public Type m_type;
        public string m_propertyName;
    }
    public CurveData[] m_curveDatas;
    public CurveClipData(int count)
    {
        m_curveDatas = new CurveData[count];
    }
}
