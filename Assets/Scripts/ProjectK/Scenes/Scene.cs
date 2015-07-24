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

        public bool Playing { get; private set; }
        private float startTime;
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }

        private List<MonsterEntity> monsters = new List<MonsterEntity>();


        public void Init()
        {
            sceneRoot = gameObject;
            loader = new ResourceLoader();

            GameObject mapRoot = new GameObject("MapRoot");
            mapRoot.transform.SetParent(sceneRoot.transform, false);

            Map = mapRoot.AddComponent<Map>();
            Map.Init(loader);

            SpawnManager = new SpawnManager();
        }

        protected override void OnDispose()
        {
            if (monsters != null)
            {
                foreach (var monster in monsters)
                    monster.Dispose();
                monsters = null;
            }

            if (Map != null)
            {
                Map.Dispose();
                Map = null;
            }

            if (loader != null)
            {
                loader.Dispose();
                loader = null;
            }

            if (sceneRoot != null)
            {
                DestroyObject(sceneRoot);
                sceneRoot = null;
            }

            base.OnDispose();
        }

        public void Load(SceneSetting setting)
        {
            Map.Load(setting.Map);
            SpawnManager.Load(setting.Spawn);
        }

        public void Load(string url)
        {
            SceneSetting setting = loader.Load<JsonFile<SceneSetting>>(url).Data;
            Load(setting);
        }

        public void StartScene()
        {
            Playing = true;
            startTime = UnityEngine.Time.fixedTime;

            SpawnManager.Start();
        }

        private void FixedUpdate()
        {
            if (!Playing)
                return;

            Time = UnityEngine.Time.fixedTime - startTime;
            DeltaTime = UnityEngine.Time.fixedDeltaTime;

            SpawnManager.Activate(this, Time);

            foreach (var monster in monsters)
                monster.Activate(this);
        }

        public void CreateMonster(int pathIndex, int templateID, int count)
        {
            MapPath path = Map.Paths[pathIndex];
            for (int i = 0; i < count; ++i)
            {
                MonsterEntity monster = SceneEntityManager.Create<MonsterEntity>(loader, templateID);
                monster.gameObject.transform.SetParent(sceneRoot.transform);
                monster.gameObject.transform.position = path.StartPosition;
                monster.SetPath(path);
                monsters.Add(monster);
            }
        }
    }
}
