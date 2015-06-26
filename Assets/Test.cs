using UnityEngine;
using System;
using System.Collections;
using Assets.Scripts.ProjectK.Settings;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Events;

namespace Assets.Scripts.ProjectK
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
            Log.Info("shit");

            var json = new { my = "little", pony = new []{0} };
            var str = SimpleJson.SerializeObject(json);
            var json2 = SimpleJson.DeserializeObject(str);
            Log.Info(str);

            ResourceLoader loader = new ResourceLoader();
            JsonFile<MapSetting> jsonFile = loader.LoadJsonFile<MapSetting>("Settings/TestMapSetting.json");
            MapSetting setting = jsonFile.Data;

            new FuckBase();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        void Update()
        {

        }

    }
}

