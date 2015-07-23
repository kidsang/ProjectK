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
        private List<HeroEntity> monsters = new List<HeroEntity>();


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

        public void LoadMap(SceneSetting setting)
        {
            Map.Load(setting.Map);
            SpawnManager.Load(setting.Spawn);
        }

        public void StartScene()
        {
            startTime = Time.fixedTime;
        }

        private void FixedUpdate()
        {
            float time = Time.fixedTime - startTime;
            SpawnManager.Activate(this, time);

            foreach (var monster in monsters)
                monster.Activate(this, time);
        }

        public void CreateHero(int x, int y, int heroID, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                HeroEntity hero = SceneEntityManager.Create<HeroEntity>(loader, heroID);
                hero.gameObject.transform.SetParent(sceneRoot.transform);
                hero.gameObject.transform.position = MapUtils.LocationToPosition(x, y);
                monsters.Add(hero);
            }
        }
    }
}
