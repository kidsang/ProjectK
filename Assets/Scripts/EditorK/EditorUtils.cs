using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.EditorK
{
    public static class EditorUtils
    {
        public static Dictionary<string, object> GetEventInfos(object[] args)
        {
            return args[0] as Dictionary<string, object>;
        }
    }
}
