using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public enum ResourceState
    {
        Loading,
        Complete,
        Disposed
    }

    public delegate void ResourceLoadComplete(Resource res);

    public abstract class Resource : Disposable
    {
        private ResourceManager manager = ResourceManager.Instance;
        private string url;
        private int refCount = 1;
        protected ResourceState state = ResourceState.Loading;
        protected bool loadFailed = false;

        internal event ResourceLoadComplete OnLoadComplete;

        internal void Init(string url)
        {
            this.url = url;
        }
        
        abstract internal void Load();

        internal void NotifyComplete()
        {
            if (OnLoadComplete != null)
            {
                OnLoadComplete(this);
                OnLoadComplete = null;
            }
        }

        internal void AddRef()
        {
            ++refCount;
        }

        internal void DecRef()
        {
            --refCount;
            if (refCount <= 0)
                base.Dispose();
        }

        [Obsolete("不要使用Resource.Dispose()，使用ResourceLoader.Dispose()", true)]
        new public void Dispose()
        {
            throw new Exception("不要使用Resource.Dispose()，使用ResourceLoader.Dispose()");
        }

        protected override void OnDispose()
        {
            OnLoadComplete = null;
            manager.RemoveResource(this);
            manager = null;
            state = ResourceState.Disposed;

            base.OnDispose();
        }

        public string Url
        {
            get { return url; }
        }

        public bool Loading
        {
            get { return state == ResourceState.Loading; }
        }

        public bool Complete
        {
            get { return state == ResourceState.Complete; }
        }

        public bool LoadFailed
        {
            get { return loadFailed; }
        }
    }
}
