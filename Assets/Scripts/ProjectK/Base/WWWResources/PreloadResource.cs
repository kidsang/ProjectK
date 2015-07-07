using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base.WWWResources
{
    public class PreloadResource : Resource
    {
        new internal IEnumerator AfterLoad()
        {
            yield break;
        }

        internal override IEnumerator OnPrepareData()
        {
            yield break;
        }
    }
}
