﻿using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Messaging
{
    public abstract class SubjectBase
    {
        public abstract void Publish(object message);
    }

    public class Subject<T> : SubjectBase
    {
        private readonly object _lock = new object();
        private readonly List<Action<T>> actions = new List<Action<T>>();

        public bool IsEmpty()
        {
            lock (_lock)
            {
                return this.actions.Count <= 0;
            }
        }

        public override void Publish(object message)
        {
            this.Publish((T)message);
        }

        public void Publish(T message)
        {
            lock (_lock)
            {
                if (actions.Count <= 0)
                    return;

                foreach (Action<T> action in this.actions)
                {
                    try
                    {
                        action(message);
                    }
                    catch (Exception) { }
                }
            }
        }

        public IDisposable Subscribe(Action<T> action)
        {
            this.Add(action);
            return new Subscription(this, action);
        }

        internal void Add(Action<T> action)
        {
            lock (_lock)
            {
                this.actions.Add(action);
            }
        }

        internal void Remove(Action<T> action)
        {
            lock (_lock)
            {
                this.actions.Remove(action);
            }
        }

        class Subscription : IDisposable
        {
            private readonly object _lock = new object();
            private Subject<T> parent;
            private Action<T> action;

            public Subscription(Subject<T> parent, Action<T> action)
            {
                this.parent = parent;
                this.action = action;
            }

            #region IDisposable Support
            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (this.disposed)
                    return;

                lock (_lock)
                {
                    try
                    {
                        if (this.disposed)
                            return;

                        if (parent != null)
                        {
                            parent.Remove(action);
                            action = null;
                            parent = null;
                        }
                    }
                    catch (Exception) { }
                    disposed = true;
                }
            }

            ~Subscription()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}
