using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ProjectK.Base
{
    public class Json
    {
    }

    public class JsonNode
    {
        private int count;
        private string value;
        private Dictionary<string, JsonNode> values;

        public static implicit operator JsonNode(string value)
        {
            JsonNode node = new JsonNode();
            node.value = value;
            return node;
        }

        public static implicit operator JsonNode(long value)
        {
            JsonNode node = new JsonNode();
            node.value = value.ToString();
            return node;
        }

        public static implicit operator JsonNode(double value)
        {
            JsonNode node = new JsonNode();
            node.value = value.ToString();
            return node;
        }

        public JsonNode this[string key]
        {
            get
            {
                if (values == null)
                    values = new Dictionary<string, JsonNode>();

                JsonNode node = null;
                values.TryGetValue(key, out node);
                return node;
            }

            set
            {
                if (values == null)
                    values = new Dictionary<string, JsonNode>();

                if (values.ContainsKey(key))
                {
                    if (value == null)
                    {
                        count -= 1;
                        values.Remove(key);
                    }
                    else
                    {
                        values[key] = value;
                    }
                }
                else if (value != null)
                {
                    count += 1;
                    values.Add(key, value);
                }
            }
        }

        public JsonNode this[int index]
        {
            get
            {
                string key = index.ToString();
                return this[key];
            }

            set
            {
                string key = index.ToString();
                this[key] = value;
            }
        }

        public bool IsDictionary
        {
            get { return values != null; }
        }

        public string Value
        {
            get { return value; }
        }

        public int AsInt
        {
            get
            {
                int ret = 0;
                int.TryParse(value, out ret);
                return ret;
            }
        }

        public float AsFloat
        {
            get
            {
                float ret = 0;
                float.TryParse(value, out ret);
                return ret;
            }
        }

        public double AsDouble
        {
            get
            {
                double ret = 0;
                double.TryParse(value, out ret);
                return ret;
            }
        }

        public double AsDouble
        {
            get
            {
                double ret = 0;
                double.TryParse(value, out ret);
                return ret;
            }
        }

        public bool AsBool
        {
            get
            {
                bool ret = false;
                bool.TryParse(value, out ret);
                return ret;
            }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool pretty, uint indent = 1)
        {
            if (values == null)
                return value;

            StringBuilder builder = new StringBuilder();
            builder.Append("{\n");

            int index = 0;
            foreach (KeyValuePair<string, JsonNode> pair in values)
            {
                if (pretty)
                {
                    for (uint i = 0; i < indent; ++i)
                        builder.Append("    ");
                }

                builder.Append("\"");
                builder.Append(pair.Key);
                builder.Append("\":");

                if (pretty)
                    builder.Append(" ");

                if (!pair.Value.IsDictionary)
                {
                    builder.Append("\"");
                    builder.Append(pair.Value.ToString());
                    builder.Append("\"");
                }
                else
                {
                    builder.Append(pair.Value.ToString(pretty, indent + 1));
                }

                if (++index < count)
                    builder.Append(",");

                if (pretty)
                    builder.Append("\n");
            }

            if (pretty)
            {
                for (uint i = 1; i < indent; ++i)
                    builder.Append("    ");
            }
            builder.Append("}");

            return builder.ToString();
        }
    }
}
