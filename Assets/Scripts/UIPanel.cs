using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MiniNGUI
{
    public class UIPanel : UIRect
    {
        static public List<UIPanel> list = new List<UIPanel>();

        [System.NonSerialized]
        public List<UIDrawCall> drawCalls = new List<UIDrawCall>();

        [HideInInspector] [SerializeField] int mDepth = 0;

        // 从这里看出如果深度相同的两个panel是无法确定先后顺序的
        static public int CompareFunc(UIPanel a, UIPanel b)
        {
            if (a != b && a != null && b != null)
            {
                if (a.mDepth < b.mDepth) return -1;
                if (a.mDepth > b.mDepth) return 1;
                return (a.GetInstanceID() < b.GetInstanceID()) ? -1 : 1;
            }
            return 0;
        }
        static int mUpdateFrame = -1;
        private void LateUpdate()
        {
            if (mUpdateFrame != Time.frameCount || !Application.isPlaying)
            {
                mUpdateFrame = Time.frameCount;
                if (Application.isPlaying)
                {

                }
            }
        }
    }

    public class SharedList<T>
    {
        class BufferInfo
        {
            public int bufferSize;
            public List<T[]> buffers;

            public BufferInfo(int bufferSize_, int capacity_)
            {
                bufferSize = bufferSize_;
                buffers = new List<T[]>(capacity_);
            }

            public T[] Get()
            {
                int count = buffers.Count;
                if (count == 0)
                {
                    return new T[bufferSize];
                }
                var p = buffers[--count];
                buffers.RemoveAt(count);
                return p;
            }

            public void Put(T[] p)
            {
                buffers.Add(p);
            }
        }

        const int cMinSize = 8;

        static BufferInfo[] bufferInfos =
        {
        new BufferInfo(4, 100),
        new BufferInfo(8, 100),
        new BufferInfo(16, 100),
        new BufferInfo(32, 50),
        new BufferInfo(64, 50),
        new BufferInfo(128, 20),
        new BufferInfo(256, 10),
    };
        static List<SharedList<T>> lists = new List<SharedList<T>>(100);

        public static SharedList<T> GetList()
        {
            int count = lists.Count;
            if (count == 0) return new SharedList<T>();

            var p = lists[--count];
            lists.RemoveAt(count);
            return p;
        }

        public static void PutList(SharedList<T> p)
        {
            var data = p.data;
            if (data != null)
            {
                int c = p.size;
                p.data = null;
                p.size = 0;
                PutBuffer(data, c);
            }
        }

        static T[] GetBuffer(int size)
        {
            int c = bufferInfos.Length;
            for (int i = 0; i < c; ++i)
            {
                var info = bufferInfos[i];
                if (size <= info.bufferSize)
                {
                    return info.Get();
                }
            }
            return new T[size];
        }

        static void PutBuffer(T[] p, int count)
        {
            if (p == null) return;
            for (int i = 0; i < count; ++i)
            {
                p[i] = default;
            }
            int size = p.Length;
            int c = bufferInfos.Length;
            for (int i = 0; i < c; ++i)
            {
                var info = bufferInfos[i];
                if (size == info.bufferSize)
                {
                    info.Put(p);
                    break;
                }
            }
        }

        void AllocateMore()
        {
            T[] newList;
            if (data != null)
            {
                newList = GetBuffer(Mathf.Max(data.Length << 1, cMinSize));
                if (size > 0) data.CopyTo(newList, 0);
                PutBuffer(data, size);
            }
            else
            {
                newList = GetBuffer(cMinSize);
            }
            data = newList;
        }

        public void Add(T item)
        {
            if (data == null || size == data.Length) AllocateMore();
            data[size++] = item;
        }

        public T[] data;
        public int size;
    }
}
