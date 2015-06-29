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
    public class GameEditor : MonoBehaviour
    {
        private static GameEditor instance;

        public EditorMap Map;
        public Canvas UICanvas;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个Editor实例！");

            ResourceManager.Init();
        }

        void Start()
        {
        }

        public static GameEditor Instance
        {
            get { return instance; }
        }
    }
}
