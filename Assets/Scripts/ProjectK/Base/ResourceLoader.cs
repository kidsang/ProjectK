using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectK.Base
{
    public class ResourceLoader : Disposable
    {
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
        private ResourceManager manager = ResourceManager.Instance;

        public IniFile LoadIniFile(string url)
        {
            return Load<IniFile>(url);
        }

        public IniFile LoadIniFileAsync(string url, ResourceLoadComplete onLoadComplete)
        {
            return LoadAsync<IniFile>(url, onLoadComplete);
        }

        public TabFile<T> LoadTabFile<T>(string url) where T : TabFileObject, new()
        {
             return Load<TabFile<T>>(url);
        }

        public TabFile<T> LoadTabFileAsync<T>(string url, ResourceLoadComplete onLoadComplete) where T : TabFileObject, new()
        {
            return LoadAsync<TabFile<T>>(url, onLoadComplete);
        }

        public PrefabResource LoadPrefab(string url)
        {
            return Load<PrefabResource>(url);
        }

        public PrefabResource LoadPrefabAsync(string url, ResourceLoadComplete onLoadComplete = null)
        {
            return LoadAsync<PrefabResource>(url, onLoadComplete);
        }

        public T Load<T>(string url) where T: Resource, new()
        {
            T res = manager.GetResource<T>(url);
            if (!res.Complete)
                res.Load();
            return res;
        }

        public T LoadAsync<T>(string url, ResourceLoadComplete onLoadComplete = null) where T: Resource, new()
        {
            T res = manager.GetResource<T>(url);
            if (onLoadComplete != null)
                res.OnLoadComplete += onLoadComplete;
            manager.AppendResource(res);
            return res;
        }

        protected override void OnDispose()
        {
            foreach (Resource res in resources.Values)
            {
                res.DecRef();
            }
            resources = null;

            manager = null;

            base.OnDispose();
        }
    }
}
