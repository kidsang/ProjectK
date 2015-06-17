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

        void Start()
        {
            Game.Init();
            Log.Info("shit");
        }

        void Update()
        {

        }

    }
}

