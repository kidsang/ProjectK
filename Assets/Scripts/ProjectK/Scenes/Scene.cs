using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Entites;
using Assets.Scripts.ProjectK.Maps;

namespace Assets.Scripts.ProjectK.Scenes
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
            mapRoot.transform.parent = sceneRoot.transform;
            map = mapRoot.AddComponent<Map>();
            map.Init(loader);

            // todo test:
            map.ResizeMap(4, 3);
            SceneEntityManager.Create(loader, 1);
        }

        protected override void OnDispose()
        {
            loader.Dispose();
            DestroyObject(sceneRoot);

            base.OnDispose();
        }
    }
}
