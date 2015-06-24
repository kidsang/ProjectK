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

            JsonNode node = new JsonNode();
            node["my"] = "little";
            node[1] = 1;
            node["pony"] = new JsonNode();
            node["pony"][0] = 0;

            String str = Json.Stringfy(node, true);
            Log.Info(str);

            JsonNode copy = Json.Parse(str);
            Log.Info(copy);
        }

        void Update()
        {

        }

    }
}

