﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK.Base.WWWResources
{
    public enum ResourceState
    {
        Downloading,
        Downloaded,
        Complete,
        Disposed
    }

    public delegate void ResourceLoadComplete(Resource res);

    public abstract class Resource : Disposable
    {
        private ResourceManager manager = ResourceManager.Instance;
        private string url;
        internal ResourcePriority priority;
        private int refCount = 1;

        private ResourceState state = ResourceState.Downloading;
        protected bool loadFailed = false;
        protected WWW www;

        internal event ResourceLoadComplete OnLoadComplete;

        internal IEnumerator Load()
        {
            www = new WWW(url);
            yield return www;
            if (Disposed)
                yield break;

            state = ResourceState.Downloaded;
            yield return AfterLoad();
        }

        internal IEnumerator AfterLoad()
        {
            // 从Preload处恢复的时候，Preload很可能还没有加载完
            while (!www.isDone) 
                yield return null;

            string error = www.error;
            if (string.IsNullOrEmpty(error))
            {
                yield return OnDownloaded();
                if (Disposed)
                    yield break;
            }
            else
            {
                loadFailed = true;
                Log.Error("资源加载错误! Url:", url, "\nType:", GetType(), "\nError:", error);
            }

            state = ResourceState.Complete;
            NotifyComplete();
        }

        abstract internal IEnumerator OnDownloaded();

        internal void NotifyComplete()
        {
            if (OnLoadComplete != null)
            {
                OnLoadComplete(this);
                OnLoadComplete = null;
            }
        }

        internal void Init(string url, ResourcePriority priority)
        {
            this.url = url;
            this.priority = priority;
        }

        internal void CopyFrom(PreloadResource preloaded)
        {
            this.url = preloaded.url;
            this.state = preloaded.state;
            this.loadFailed = preloaded.loadFailed;

             // 把preload的www的所有权转移给自己
            this.www = preloaded.www;
            preloaded.www = null;
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

            if (www != null)
            {
                www.Dispose();
                www = null;
            }

            manager.RemoveResource(this);
            manager = null;
            state = ResourceState.Disposed;

            base.OnDispose();
        }

        public string Url
        {
            get { return url; }
        }

        public bool Downloading
        {
            get { return state == ResourceState.Downloading; }
        }

        public bool Downloaded
        {
            get { return state == ResourceState.Downloaded; }
        }

        public bool Complete
        {
            get { return state == ResourceState.Complete; }
        }

        public bool LoadFailed
        {
            get { return loadFailed; }
        }

        public float Progress
        {
            get { return www != null ? www.progress : 0; }
        }
    }
}
