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
        public string Name;
        public float Width;
        public float Height;
        public string Prefab;

        public override string GetKey()
        {
            return ID.ToString();
        }
    }
}
