using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Entites;

namespace Assets.Scripts.ProjectK.Settings
{
    public class EntitySetting : TabFileObject
    {
        public int ID;
        public EntityType Type;
        public float Width;
        public float Height;

        public override string GetKey()
        {
            return ID.ToString();
        }
    }
}
