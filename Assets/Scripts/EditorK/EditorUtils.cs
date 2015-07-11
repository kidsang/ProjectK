using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorK
{
    public class InfoMap : Dictionary<string, object>
    {
    }

    public static class EditorUtils
    {
        public static InfoMap GetEventInfos(object[] args)
        {
            return args[0] as InfoMap;
        }
    }
}
