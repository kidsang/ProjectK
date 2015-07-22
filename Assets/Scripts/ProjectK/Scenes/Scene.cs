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
        public Map Map { get; private set; }
        public SpawnManager SpawnManager { get; private set; }

        private float startTime;

        public void Init()
        {
            sceneRoot = gameObject;
            loader = new ResourceLoader();

            GameObject mapRoot = new GameObject("MapRoot");
            mapRoot.transform.SetParent(sceneRoot.transform, false);

            Map = mapRoot.AddComponent<Map>();
            Map.Init(loader);

            SpawnManager = new SpawnManager();
            SpawnManager.Init(this);
        }

        protected override void OnDispose()
        {
            Map.Dispose();
            Map = null;

            loader.Dispose();
            loader = null;

            DestroyObject(sceneRoot);
            sceneRoot = null;

            base.OnDispose();
        }

        public void LoadMap(SceneSetting setting)
        {
            Map.Load(setting.Map);
            SpawnManager.Load(setting.Spawns);
        }

        public void StartScene()
        {
            startTime = Time.fixedTime;
        }

        private void FixedUpdate()
        {
            float time = Time.fixedTime - startTime;
            SpawnManager.Activate(time);
        }
    }
}
