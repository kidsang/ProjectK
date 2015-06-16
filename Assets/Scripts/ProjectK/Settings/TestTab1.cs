using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK.Settings
{
    class TestTab1 : TabFileObject
    {
        public int ID;
        public string Value;

        public override string GetKey()
        {
            return ID.ToString();
        }
    }
}
