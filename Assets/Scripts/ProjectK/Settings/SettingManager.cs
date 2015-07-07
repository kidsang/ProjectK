using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
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
            LoadTabFile<EntitySetting>("Settings/SceneEntities.tab", out EntitySettings);
        }

        private void LoadIniFile(string url, out IniFile res)
        {
            ++loadingCount;
            res = loader.LoadIniFileAsync(url, OnLoadComplete);
        }

        private void LoadTabFile<T>(string url, out TabFile<T> res) where T: TabFileObject, new()
        {
            ++loadingCount;
            res = loader.LoadTabFileAsync<T>(url, OnLoadComplete);
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
