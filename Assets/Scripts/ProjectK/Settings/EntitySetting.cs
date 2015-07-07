using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace ProjectK
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
