using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MiniNGUI
{
    public class UIRect : MonoBehaviour
    {
        [NonSerialized] protected Transform mTrans;
        public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
    }

}
