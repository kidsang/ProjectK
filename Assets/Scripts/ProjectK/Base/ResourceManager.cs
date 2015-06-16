using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectK.Base
{
    public class ResourceManager : MonoBehaviour
    {
        private static ResourceManager instance;

        private string resRoot;

        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

        public static void Init(string resRoot)
        {
            if (instance)
                return;

            GameObject gameObject = new GameObject("ResourceManagerObject");
            DontDestroyOnLoad(gameObject);
            instance = gameObject.AddComponent<ResourceManager>();

            if (resRoot[resRoot.Length - 1] != '/')
                resRoot += "/";
            instance.resRoot = resRoot;
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
            T res;
            if (resources.ContainsKey(url))
            {
                Resource tempRes = resources[url];
                if (typeof(T) != tempRes.GetType())
                    Log.Assert(false, "使用不同的类型加载了同一份资源！ url:", url, "oldType:", tempRes.GetType(), "newType:", typeof(T));
                res = (T)tempRes;
                res.AddRef();
            }
            else
            {
                res = new T();
                res.Init(url);
                StartCoroutine(res.Load());
            }
            return res;
        }

        internal void RemoveResource(Resource res)
        {
            resources.Remove(res.Url);
        }

        public string ResRoot
        {
            get { return resRoot; }
        }
    }
}
