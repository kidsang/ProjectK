using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK.Entites
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
