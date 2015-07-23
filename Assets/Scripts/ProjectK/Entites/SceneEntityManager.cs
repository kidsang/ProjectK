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
        public static T Create<T>(ResourceLoader loader, int templateID) where T: SceneEntity
        {
            EntitySetting template = GetEntitySetting<T>(templateID);

            GameObject gameObject;
            if (string.IsNullOrEmpty(template.Prefab))
                gameObject = new GameObject();
            else
                gameObject = loader.LoadPrefab(template.Prefab).Instantiate();

            T entity = gameObject.AddComponent<T>();
            entity.Init(loader, template);
            return entity;
        }

        public static EntitySetting GetEntitySetting<T>(int templateID) where T : SceneEntity
        {
            Type type = typeof(T);
            SettingManager settings = SettingManager.Instance;

            EntitySetting setting;
            if (type == typeof(MonsterEntity))
                setting = settings.MonsterEntitySettings.GetValue(templateID);
            else
                setting = null;

            if (setting == null)
                Log.Assert(false, "找不到场景物件定义EntitySetting! Type:", type, "ID:", templateID);

            return setting;
        }
    }
}
