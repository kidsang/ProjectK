using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class GameEditor : DisposableBehaviour
    {
        private static GameEditor instance;
        public static GameEditor Instance { get { return instance; } }

        private GameObject sceneRoot;
        public EditorMap Map;
        public Canvas UICanvas;

        public override void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个GameEditor实例！");

            sceneRoot = gameObject;
            ResourceManager.Init();
        }

        void Start()
        {
            NewMap();
        }

        public void NewMap()
        {
            MapSetting data = new MapSetting();
            data.CellCountX = 10;
            data.CellCountY = 10;
            LoadMap(data);
        }

        public void LoadMap(MapSetting data, string path = null)
        {
            if (Map)
                Map.Dispose();

            GameObject mapRoot = new GameObject("MapRoot");
            mapRoot.transform.SetParent(sceneRoot.transform);
            mapRoot.transform.SetSiblingIndex(0);

            Map = mapRoot.AddComponent<EditorMap>();
            Map.New(data);

            MapDataProxy.Instance.Load(data, path);
        }
    }
}
