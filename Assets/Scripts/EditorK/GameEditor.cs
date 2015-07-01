using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.EditorK.Maps;

namespace Assets.Scripts.EditorK
{
    public class GameEditor : DisposableBehaviour
    {
        private static GameEditor instance;
        public static GameEditor Instance { get { return instance; } }

        public EditorMap Map;
        public Canvas UICanvas;

        public override void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个GameEditor实例！");

            ResourceManager.Init();
        }

        void Start()
        {
        }

    }
}
