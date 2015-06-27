using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.EditorK
{
    public class Editor : MonoBehaviour
    {
        private static Editor instance;

        void Start()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个Editor实例！");

            ResourceManager.Init();
        }

        void Update()
        {
            Log.Info(Screen.width, Screen.height);
        }
    }
}
