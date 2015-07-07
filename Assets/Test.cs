using UnityEngine;
using System;
using System.Collections;
using ProjectK.Base;

namespace ProjectK
{
    public class FuckBase : Disposable
    {
        public FuckBase()
        {
        }
    }

    public class Test : DisposableBehaviour
    {

        void Start()
        {
            Game.Init();

            ResourceLoader loader = new ResourceLoader();
            JsonFile<MapSetting> jsonFile = loader.LoadJsonFile<MapSetting>("Settings/TestMapSetting.json");
            MapSetting setting = jsonFile.Data;
            var str = SimpleJson.SerializeObject(setting);
            Log.Info(str);

            new FuckBase();

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        void Update()
        {
        }

    }
}

