using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK.Base
{
    public class JsonFile<T> : TextResource
    {
        private T data;

        internal override void Load()
        {
            base.Load();
            if (Text != null)
                data = SimpleJson.DeserializeObject<T>(Text);
        }

        public void LoadFromData(string rawData)
        {
            data = SimpleJson.DeserializeObject<T>(rawData);
        }

        public T Data
        {
            get { return data; }
        }
    }

    public class JsonFile : TextResource
    {
        private JsonObject data;

        internal override void Load()
        {
            base.Load();
            if (Text != null)
                data = SimpleJson.DeserializeObject(Text) as JsonObject;
        }

        public void LoadFromData(string rawData)
        {
            data = SimpleJson.DeserializeObject(rawData) as JsonObject;
        }

        public JsonObject Data
        {
            get { return data; }
        }
    }
}
