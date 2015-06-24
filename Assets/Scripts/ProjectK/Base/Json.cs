using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ProjectK.Base
{
    public enum JsonNodeType
    {
        Value,
        Array,
        Dictionary,
    }

    public class JsonNode
    {
        private int count;
        private string value;
        private Dictionary<string, JsonNode> values;
        private JsonNodeType type;

        public JsonNode()
        {
            type = JsonNodeType.Value;
        }

        public JsonNode(JsonNodeType type)
        {
            this.type = type;
        }

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
                if (type == JsonNodeType.Value)
                    type = JsonNodeType.Dictionary;

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
                if (type == JsonNodeType.Value)
                    type = JsonNodeType.Array;

                string key = index.ToString();
                this[key] = value;
            }
        }

        public bool IsArray
        {
            get { return type == JsonNodeType.Array; }
        }

        public bool IsDictionary
        {
            get { return type == JsonNodeType.Dictionary; }
        }

        public int Count
        {
            get { return count; }
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

        public bool AsBool
        {
            get
            {
                bool ret = false;
                bool.TryParse(value, out ret);
                return ret;
            }
        }

        public T AsEnum<T>() where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            Log.Assert(enumType.IsEnum);

            if (string.IsNullOrEmpty(value))
                return default(T);

            return (T)Enum.Parse(enumType, value);
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

    public class Json
    {
        public static string Stringfy(JsonNode node, bool pretty = false)
        {
            return node.ToString(pretty);
        }

        public static JsonNode Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            JsonNode node = null;
            Stack<JsonNode> stack = new Stack<JsonNode>();
            int cur = 0;
            int begin = 0;
            int length = data.Length;
            bool quoted = false;
            string key = null;
            string value = null;

            while (cur < length)
            {
                switch (data[cur])
                {
                    case '{':
                        if (!quoted)
                        {
                            stack.Push(new JsonNode(JsonNodeType.Dictionary));
                            if (node != null)
                            {
                                key = data.Substring(begin, cur - begin + 1);
                                AddChild(node, key, stack.Peek());
                            }

                            key = null;
                            begin = cur + 1;
                            node = stack.Peek();
                        }
                        break;

                    case '[':
                        if (!quoted)
                        {
                            stack.Push(new JsonNode(JsonNodeType.Array));
                            if (node != null)
                            {
                                key = data.Substring(begin, cur - begin + 1);
                                AddChild(node, key, stack.Peek());
                            }

                            key = null;
                            begin = cur + 1;
                            node = stack.Peek(); ;
                        }
                        break;

                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            if (stack.Count == 0)
                                Log.Assert(false, "Json解析错误：过多的闭合括号 \"", data[cur], "\" 。");

                            stack.Pop();

                            value = data.Substring(begin, cur - begin + 1);
                            AddValue(node, key, value);
                            key = null;
                            value = null;
                            begin = cur + 1;

                            if (stack.Count > 0)
                                node = stack.Peek();
                        }
                        break;

                    case ':':
                        if (!quoted)
                        {
                            key = data.Substring(begin, cur - begin + 1);
                            begin = cur + 1;
                        }
                        break;

                    case '"':
                        quoted = !quoted;
                        break;

                    case ',':
                        if (!quoted)
                        {
                            AddValue(node, key, value);
                            key = null;
                            value = null;
                            begin = cur + 1;
                        }
                        break;

                    case '\r':
                    case '\n':
                        begin = cur + 1;
                        break;

                    case ' ':
                    case '\t':
                        if (!quoted)
                            begin = cur + 1;
                        break;

                    default:
                        break;
                }
                ++cur;
            }

            if (quoted)
                Log.Assert(false, "Json解析错误：双引号错误。");

            return node;
        }

        private static void AddChild(JsonNode node, string key, JsonNode child)
        {
            if (child == null)
                return;

            if (node.IsDictionary)
            {
                if (key == null)
                    Log.Assert(false, "Json解析错误：空键。value: JsonNode");
                node[key] = child;
            }
            else if (node.IsArray)
            {
                node[node.Count] = child;
            }
        }

        private static void AddValue(JsonNode node, string key, string value)
        {
            if (value == null)
                return;

            if (node.IsDictionary)
            {
                if (key == null)
                    Log.Assert(false, "Json解析错误：空键。value:", value);
                node[key] = value;
            }
            else if (node.IsArray)
            {
                node[node.Count] = value;
            }
        }
    }
}
