using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectK.Base
{
    enum ResourceState
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
        private string fullUrl;
        private int refCount = 1;
        private ResourceState state = ResourceState.Loading;
        private bool loadFailed = false;

        protected WWW www;
        internal event ResourceLoadComplete OnLoadComplete;

        internal void Init(string url)
        {
            this.url = url;
            fullUrl = manager.ResRoot + url;
        }

        internal IEnumerator Load()
        {
            www = new WWW(fullUrl);
            yield return www;

            string error = www.error;
            if (string.IsNullOrEmpty(error))
            {
                OnPrepareResource();
            }
            else
            {
                loadFailed = true;
                Log.Error("资源加载错误! Url:", url, "\nType:", GetType(), "\nFullUrl:", fullUrl, "\nError:", error);
            }

            state = ResourceState.Complete;
            NotifyComplete();
        }

        abstract protected void OnPrepareResource();

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
            www.Dispose();
            www = null;
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
