using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class ResourceManager : MonoBehaviour
    {
        private static ResourceManager instance;

        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
        private Queue<Resource> loadingResources = new Queue<Resource>();

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
        internal T GetResource<T>(string url) where T: Resource, new()
        {
            Resource res;
            if (resources.TryGetValue(url, out res))
            {
                if (typeof(T) == res.GetType())
                {
                    res.AddRef();
                    return (T)res;
                }
                else
                {
                    Log.Warning("使用不同的类型加载了同一份资源！缓存被更新！ url:", url, "oldType:", res.GetType(), "newType:", typeof(T));
                }
            }

            res = new T();
            res.Init(url);
            resources[url] = res;
            return (T)res;
        }

        internal void RemoveResource(Resource res)
        {
            resources.Remove(res.Url);
        }

        internal void AppendResource(Resource res)
        {
            loadingResources.Enqueue(res);
        }

        void Update()
        {
            while (loadingResources.Count > 0)
            {
                Resource res = loadingResources.Dequeue();
                if (res.Disposed)
                    continue;

                if (res.Complete)
                {
                    res.NotifyComplete();
                }
                else
                {
                    res.Load();
                    res.NotifyComplete();
                }
                break;
            }
        }
    }
}
