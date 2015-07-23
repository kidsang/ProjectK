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

        public TabFile<MonsterEntitySetting> MonsterEntitySettings;

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
            MonsterEntitySettings = LoadTabFile<MonsterEntitySetting>("Settings/MonsterEntities.tab");
        }

        private IniFile LoadIniFile(string url)
        {
            ++loadingCount;
            return loader.LoadIniFileAsync(url, OnLoadComplete);
        }

        private TabFile<T> LoadTabFile<T>(string url) where T: TabFileObject, new()
        {
            ++loadingCount;
            return loader.LoadTabFileAsync<T>(url, OnLoadComplete);
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
