using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK.Settings
{
    public class SettingManager
    {
        private static SettingManager instance;

        private ResourceLoader loader;
        private int loadingCount;
        private int completeCount;

        public delegate void AllCompleteCallback();
        private AllCompleteCallback allComplete;

        public TabFile<EntitySetting> EntitySettings;

        public static void Init(AllCompleteCallback allComplete)
        {
            if (instance != null)
                return;

            instance = new SettingManager();
            instance.loader = new ResourceLoader();
            instance.allComplete = allComplete;
            instance.LoadAll();
        }

        private void LoadAll()
        {
            LoadTabFile<EntitySetting>("SceneEntities.tab", out EntitySettings);
        }

        private void LoadIniFile(string url, out IniFile res)
        {
            ++loadingCount;
            loader.LoadIniFile(url, out res, OnLoadComplete);
        }

        private void LoadTabFile<T>(string url, out TabFile<T> res) where T: TabFileObject, new()
        {
            ++loadingCount;
            loader.LoadTabFile(url, out res, OnLoadComplete);
        }

        private void OnLoadComplete(Resource res)
        {
            ++completeCount;

            if (loadingCount == completeCount)
            {
                allComplete();
                allComplete = null;
            }
        }

        public static SettingManager Instance
        {
            get { return instance; }
        }
    }
}
