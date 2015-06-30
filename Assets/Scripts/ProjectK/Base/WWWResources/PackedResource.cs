using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectK.Base.WWWResources
{
    public class PackedResource : Resource
    {
        private Dictionary<string, byte[]> subResourceBytes = new Dictionary<string, byte[]>();
        private List<Resource> subResources = new List<Resource>();

        internal void AddSubResource(Resource subRes)
        {
            subResources.Add(subRes);
        }

        internal override IEnumerator OnDownloaded()
        {
            yield break;
            // not implement
        }

        internal void DoSubResources()
        {
            // not implement
        }

        protected override void OnDispose()
        {
            subResourceBytes = null;
            subResources = null;

            base.OnDispose();
        }
    }
}
