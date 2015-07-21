using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class Scene : DisposableBehaviour
    {
        private GameObject sceneRoot;
        private ResourceLoader loader;
        private Map map;

        internal void Init()
        {
            sceneRoot = gameObject;
            loader = new ResourceLoader();

            GameObject mapRoot = new GameObject("MapRoot");
            mapRoot.transform.SetParent(sceneRoot.transform, false);
            map = mapRoot.AddComponent<Map>();
            map.Init(loader);

            // todo test:
            //map.ResizeMap(4, 4);
            SceneEntityManager.Create<HeroEntity>(loader, 0);
        }

        protected override void OnDispose()
        {
            map.Dispose();
            map = null;

            loader.Dispose();
            loader = null;

            DestroyObject(sceneRoot);
            sceneRoot = null;

            base.OnDispose();
        }
    }
}
