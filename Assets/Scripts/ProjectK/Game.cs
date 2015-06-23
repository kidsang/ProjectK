using UnityEngine;
using System.Collections;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Settings;
using Assets.Scripts.ProjectK.Scenes;

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

            instance = new Game();
            string resRoot = "file://" + Application.dataPath;
            ResourceManager.Init(resRoot);
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
