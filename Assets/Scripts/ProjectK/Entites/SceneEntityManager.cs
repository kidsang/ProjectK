using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class SceneEntityManager
    {
        public static SceneEntity Create(ResourceLoader loader, int templateID)
        {
            GameObject gameObject = new GameObject();
            SceneEntity entity = gameObject.AddComponent<SceneEntity>();
            entity.Init(loader, templateID);
            return entity;
        }
    }
}
