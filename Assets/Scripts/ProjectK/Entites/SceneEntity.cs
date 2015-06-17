using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Settings;

namespace Assets.Scripts.ProjectK.Entites
{
    public class SceneEntity : DisposableBehaviour
    {
        private ResourceLoader loader;
        private int templateID;
        private EntitySetting template;

        internal void Init(ResourceLoader loader, int templateID)
        {
            this.loader = loader;
            this.templateID = templateID;
            template = SettingManager.Instance.EntitySettings.GetValue(templateID);
            Log.Assert(template != null, "找不到场景物件定义！ ID:", templateID);

            Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
            body.isKinematic = true;

            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(template.Width, template.Height);
            collider.isTrigger = true;
        }

    }
}
