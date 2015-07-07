using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class SceneManager
    {
        private static SceneManager instance;

        private Scene current;

        public static void Init()
        {
            if (instance != null)
                return;

            instance = new SceneManager();
        }

        public static SceneManager Instance
        {
            get { return instance; }
        }

        public void SwitchTo()
        {
            if (current != null)
                current.Dispose();

            GameObject sceneRoot = new GameObject("SceneRoot");
            current = sceneRoot.AddComponent<Scene>();
            current.Init();
        }
    }
}
