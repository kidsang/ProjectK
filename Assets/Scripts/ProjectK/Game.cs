using UnityEngine;
using System.Collections;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK
{
    public class Game
    {
        private static Game instance;

        private Game()
        {
            Log.Debug("Game init.");
        }

        public static void Init()
        {
            if (instance != null)
                return;

            string resRoot = "file://" + Application.dataPath + "/Resource";
            ResourceManager.Init(resRoot);
            instance = new Game();
        }

        public string Test()
        {
            return "lalala";
        }

        public static Game Instance
        {
            get { return instance; }
        }
    }
}
