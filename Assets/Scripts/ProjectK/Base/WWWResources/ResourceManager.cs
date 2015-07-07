using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK.Base.WWWResources
{
    public enum ResourcePriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
    }

    public class ResourceManager : MonoBehaviour
    {
        private static ResourceManager instance;

        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

        private Resource[] loadingResources = new Resource[4];
        private List<Resource> pendingResources = new List<Resource>();

        public static void Init()
        {
            if (instance)
                return;

            GameObject gameObject = new GameObject("ResourceManagerObject");
            DontDestroyOnLoad(gameObject);
            instance = gameObject.AddComponent<ResourceManager>();
        }

        public static ResourceManager Instance
        {
            get
            {
                Log.Assert(instance != null);
                return instance;
            }
        }

        // 根据T创建一个新的Resource，或者返回已有的Resource
        internal T LoadResource<T>(string url, ResourcePriority priority) where T : Resource, new()
        {
            Resource res;
            if (resources.TryGetValue(url, out res))
            {
                Type newType = typeof(T);
                Type oldType = res.GetType();

                if (newType == oldType)
                {
                    res.AddRef();
                    if (res.priority < priority)
                    {
                        res.priority = priority;
                        int index = pendingResources.IndexOf(res);
                        if (index > 0) // 如果index == 0，说明已经在队列的最前了，不需要重排
                        {
                            pendingResources.RemoveAt(index);
                            AddToPendingList(res);
                        }
                    }
                    return (T)res;
                }

                if (newType == typeof(PreloadResource))
                {
                    res.AddRef();
                    return (T)res;
                }

                if (oldType == typeof(PreloadResource))
                {
                    PreloadResource preloaded = res as PreloadResource;
                    pendingResources.Remove(preloaded);

                    res = new T();
                    res.CopyFrom(preloaded);
                    res.priority = priority;
                    resources[url] = res;
                    StartCoroutine(res.AfterLoad());
                    return (T)res;
                }

                Log.Warning("使用不同的类型加载了同一份资源！缓存被更新！ url:", url, "\n  oldType:", oldType, "\n  newType:", newType);
            }

            res = new T();
            res.Init(url, priority);
            resources[url] = res;

            if (!PackedResourceManager.Instance.TryLoad(res))
                AddToPendingList(res);
            return (T)res;
        }

        internal void RemoveResource(Resource res)
        {
            resources.Remove(res.Url);
        }

        private void AddToPendingList(Resource res)
        {
            for (int i = pendingResources.Count - 1; i >= 0; --i)
            {
                if (pendingResources[i].priority >= res.priority)
                {
                    pendingResources.Insert(i + 1, res);
                    return;
                }
            }

            pendingResources.Insert(0, res);
        }

        void Update()
        {
            int index = -1;
            for (int i = loadingResources.Length - 1; i >= 0; --i)
            {
                Resource res = loadingResources[i];
                if (res == null || !res.Downloading)
                {
                    loadingResources[i] = null;
                    index = i;
                }
            }

            if (index >= 0)
            {
                while (pendingResources.Count > 0)
                {
                    Resource res = pendingResources[0];
                    pendingResources.RemoveAt(0);
                    if (!res.Pending)
                        continue;

                    loadingResources[index] = res;
                    StartCoroutine(res.Load());
                    break;
                }
            }
        }
    }
}
