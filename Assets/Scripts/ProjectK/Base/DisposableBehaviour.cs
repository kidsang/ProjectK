using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ProjectK.Base
{
    public class DisposableBehaviour : MonoBehaviour, IDisposable
    {
        private bool disposed = false;
        private bool onDisposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            OnDispose();
            disposed = true;

            if (!onDisposed)
                Log.Error("SubClass did not call SuperClass.OnDispose()!");
        }

        virtual protected void OnDispose()
        {
            onDisposed = true;
        }

        public bool Disposed
        {
            get { return disposed; }
        }
    }
}
