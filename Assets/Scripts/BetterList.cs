using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MiniNGUI
{
    public class BetterList<T>
    {
        public T[] buffer;
        public int size = 0;
        public T this[int i]
        {
            get { return buffer[i]; }
            set { buffer[i] = value; }
        }
        public void Add(T item)
        {
            if (buffer == null || size == buffer.Length)
            {

            }
        }
        void AllocateMore()
        {
            T[] newList;
            if (buffer != null)
            {

            }
        }
    }

}
