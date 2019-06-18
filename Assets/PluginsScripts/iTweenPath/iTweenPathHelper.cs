using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PathMoveType
{
    Loop,
    PingPong,
    Once
}

// 现在不再是客户端游戏了，这个类存在的意义不大了;
//public class iTweenPathHelper : MonoBehaviour 
//{
//    public string pathName;
//    public PathMoveType moveType;
//    public float startWaitTime;
//    public bool towardPath;
//
//    void Start()
//    {
//        mFly = new PathFly(transform, pathName, (int)moveType, startWaitTime, towardPath);
//        Vector3[] mPath = iTweenPath.GetPath(pathName);
//        if (mPath == null)
//            return;
//        mFly.PathActions = new List<System.Action<int>>();
//        mFly.Enter();
//    }
//
//    void Update()
//    {
//        if (!mFly.RenderTick())
//        {
//            mFly.Exit();
//            mFly = null;
//        }
//        if (mCurModifier != null)
//        {
//            if (!mCurModifier.Update())
//            {
//                mCurModifier = null;
//            }
//        }
//    }
//
//
//    PathFly mFly;
//    CameraTargetMotifier mCurModifier;
//}
