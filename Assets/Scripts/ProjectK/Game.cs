using UnityEngine;
using System.Collections;
using ProjectK.Base;

namespace ProjectK
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

            instance = new Game();
            ResourceManager.Init();
            SceneManager.Init();
            SettingManager.Init(instance.StartGame);
        }

        private void StartGame()
        {
            Log.Debug("Start game!");
            SceneManager.Instance.SwitchTo();
        }

        public static Game Instance
        {
            get { return instance; }
        }
    }
}
