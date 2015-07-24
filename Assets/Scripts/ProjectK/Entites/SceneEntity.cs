using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
{
    public enum EntityType
    {
        Invalid = 0,
        Monster,
        Turret,
        Bullet,
    }

    public class SceneEntity : DisposableBehaviour
    {
        public ResourceLoader Loader { get; private set; }
        public EntitySetting Template { get; private set; }
        public int TemplateID { get; private set; }
        public EntityType Type { get; protected set; }

        public virtual void Init(ResourceLoader loader, EntitySetting template)
        {
            Loader = loader;
            Template = template;
            TemplateID = template.ID;

            Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
            body.isKinematic = true;

            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(Template.Width, Template.Height);
            collider.isTrigger = true;
        }

        public virtual void Activate(Scene scene)
        {

        }

        public Vector2 Location
        {
            get { return MapUtils.PositionToLocation(gameObject.transform.position); }
        }
    }
}
