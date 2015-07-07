using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base.WWWResources
{
    public class PackedResource : Resource
    {
        private Dictionary<string, byte[]> subResourceBytes = new Dictionary<string, byte[]>();
        private List<Resource> subResources = new List<Resource>();

        internal void AddSubResource(Resource subRes)
        {
            subResources.Add(subRes);
        }

        internal override IEnumerator OnPrepareData()
        {
            yield break;
            // not implement
        }

        internal IEnumerator DoSubResources()
        {
            List<Resource> list = new List<Resource>(subResources);
            subResources.Clear();

            foreach (Resource subRes in list)
            {
                if (!subRes.Disposed)
                {
                    byte[] bytes = subResourceBytes[subRes.Url];
                    yield return subRes.OnDownloaded(bytes);
                }
            }
        }

        protected override void OnDispose()
        {
            subResourceBytes = null;
            subResources = null;

            base.OnDispose();
        }
    }
}
