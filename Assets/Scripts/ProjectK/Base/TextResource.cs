using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace ProjectK.Base
{
    public class TextResource : Resource
    {
        private string text;

        internal override void Load()
        {
            try
            {
                string fullUrl = Application.dataPath + "/" + Url;
                using (StreamReader reader = new StreamReader(fullUrl))
                {
                    text = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                loadFailed = true;
                Log.Error("资源加载错误! Url:", Url, "\nType:", GetType(), "\nError:", e.Message);
            }

            state = ResourceState.Complete;
        }

        protected override void OnDispose()
        {
            text = null;
            base.OnDispose();
        }

        public string Text
        {
            get { return text; }
        }
    }
}
