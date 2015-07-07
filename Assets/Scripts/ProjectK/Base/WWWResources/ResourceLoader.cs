using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK.Base.WWWResources
{
    public class ResourceLoader : Disposable
    {
        private ResourceManager manager = ResourceManager.Instance;
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

        private bool batching;
        private List<Resource> batchingResources = new List<Resource>();


        public void Load<T>(out T res, string url, ResourceLoadComplete onLoadComplete = null, ResourcePriority priority = ResourcePriority.Normal) where T : Resource, new()
        {
            if (resources.ContainsKey(url))
            {
                res = (T)resources[url];
            }
            else
            {
                res = manager.LoadResource<T>(url, priority);
                resources[url] = res;

                if (batching)
                    batchingResources.Add(res);
            }

            if (onLoadComplete != null)
            {
                res.OnLoadComplete += onLoadComplete;
                if (res.Complete)
                    res.NotifyComplete();
            }
        }

        public void Preload(string url)
        {
            PreloadResource res;
            Load<PreloadResource>(out res, url, null, ResourcePriority.Low);
        }

        public void BeginBatch()
        {
            batching = true;
            batchingResources.Clear();
        }

        public void EndBatch()
        {
            batching = false;
        }

        public float Progress
        {
            get
            {
                float total = 0;
                float completed = 0;
                foreach (Resource res in batchingResources)
                {
                    total += 1;
                    completed += res.Progress;
                }

                if (total == 0)
                    return 0;
                return completed / total;
            }
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
