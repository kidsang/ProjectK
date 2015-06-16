using UnityEngine;
using System;
using System.Collections;
using Assets.Scripts.ProjectK.Settings;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Events;

namespace Assets.Scripts.ProjectK
{
    public class Test : DisposableBehaviour
    {
        private ResourceLoader loader;
        void Start()
        {
            Game.Init();

            //EventManager.Instance.Register(this, "event", OnEvent);
            //EventManager.Instance.Register(this, "event", OnEvent2);
            //EventManager.instance.Unregister(this, "event", OnEvent);
            //EventManager.instance.Unregister(this, "event", OnEvent2);
            //EventManager.Instance.UnregisterAll(this);
            //EventManager.Instance.FireEvent("event", 1, "2");

            loader = new ResourceLoader();

            IniFile iniFile;
            loader.LoadIniFile("test.ini", out iniFile);

            TabFile<TestTab1> tabFile;
            loader.LoadTabFile("test.tab", out tabFile);

            Log.Info("aaa");
        }

        void Update()
        {

        }

        private void OnEvent(params object[] args)
        {
            Log.Info("OnEvent:", args[0], args[1]);
        }

        private void OnEvent2(params object[] args)
        {
            Log.Info("OnEvent2:", args[0], args[1]);
        }

    }
}

