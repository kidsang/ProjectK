using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK.Base.WWWResources
{
    public class PackedResourceManager
    {
        private static PackedResourceManager instance = new PackedResourceManager();
        public static PackedResourceManager Instance { get { return instance; } }

        private ResourceLoader loader = new ResourceLoader();

        public string GetPackedUrl(string url)
        {
            return null;
            // not implement
        }

        public bool IsPackedResource(string url)
        {
            return GetPackedUrl(url) != null;
        }

        public bool TryLoad(Resource subRes)
        {
            string url = GetPackedUrl(subRes.Url);
            if (url == null)
                return false;

            PackedResource res;
            loader.Load<PackedResource>(out res, url, OnLoadComplete, subRes.priority);

            res.AddSubResource(subRes);

            if (res.Complete)
                OnLoadComplete(res);

            return true;
        }

        private void OnLoadComplete(Resource res)
        {
            PackedResource packedRes = res as PackedResource;
            if (packedRes.LoadFailed)
                return;

            packedRes.DoSubResources();
        }

        public void Clear()
        {
            loader.Dispose();
            loader = new ResourceLoader();
        }
    }
}
