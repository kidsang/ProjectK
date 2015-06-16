using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ProjectK.Base
{
    public class ResourceLoader : Disposable
    {
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
        private ResourceManager manager = ResourceManager.Instance;

        public void LoadIniFile(string url, out IniFile res, ResourceLoadComplete onLoadComplete = null)
        {
            Load<IniFile>(url, out res, onLoadComplete);
        }

        public void LoadIniFile(string url, ResourceLoadComplete onLoadComplete)
        {
            IniFile res;
            Load<IniFile>(url, out res, onLoadComplete);
        }

        public void LoadTabFile<T>(string url, out TabFile<T> res, ResourceLoadComplete onLoadComplete = null) where T: TabFileObject, new()
        {
            Load<TabFile<T>>(url, out res, onLoadComplete);
        }

        public void LoadTabFile<T>(string url, ResourceLoadComplete onLoadComplete) where T: TabFileObject, new()
        {
            TabFile<T> res;
            Load<TabFile<T>>(url, out res, onLoadComplete);
        }

        // 使用引用参数返回，因此可以在返回后调用res.NotifyComplete，避免时序问题。
        public void Load<T>(string url, out T res, ResourceLoadComplete onLoadComplete = null) where T: Resource, new()
        {
            res = manager.GetResource<T>(url);
            if (onLoadComplete != null)
                res.OnLoadComplete += onLoadComplete;

            if (res.Complete)
                res.NotifyComplete();
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
