using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ProjectK.Base
{
    public abstract class TabFileObject
    {
        private static StringBuilder keyBuilder = new StringBuilder();

        public abstract string GetKey();

        public virtual void OnComplete()
        {
        }

        public static string buildMultiKey(params object[] keys)
        {
            keyBuilder.Remove(0, keyBuilder.Length);

            int end = keys.Length - 1;
            for (int i = 0; i <= end; ++i)
            {
                keyBuilder.Append(keys[i]);
                if (i < end)
                    keyBuilder.Append("-");
            }

            return keyBuilder.ToString();
        }
    }

    public class TabFile<T> : TextResource where T: TabFileObject, new()
    {
        private static char[] LINE_SEPERATOR = new char[] { '\n', '\r' };
        private static char[] VALUE_SEPERATOR = new char[] { '\t' };

        private bool parsed = false;
        private string rawData;
        private Dictionary<string, T> datas = new Dictionary<string,T>();

        internal override void Load()
        {
            base.Load();
            if (Text != null)
                LoadFromData(Text);
        }

        public void LoadFromData(string rawData, bool parseImmediately = true)
        {
            this.rawData = rawData;
            if (parseImmediately)
                Parse();
        }

        public T GetValue(string key)
        {
            if (!parsed)
                Parse();

            T data;
            datas.TryGetValue(key, out data);
            return data;
        }

        public T GetValue(object key)
        {
            return GetValue(key.ToString());
        }

        public T GetValue(params object[] keys)
        {
            string key = TabFileObject.buildMultiKey(keys);
            return GetValue(key);
        }

        private void Parse()
        {
            if (rawData == null)
                return;

            string[] lines = rawData.Split(LINE_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
            int numLines = lines.Length;
            if (numLines <= 1)
                return;

            string[] titles = lines[0].Split(VALUE_SEPERATOR);
            int numTitles = titles.Length;
            FieldInfo[] fields = new FieldInfo[numTitles];
            Type type = typeof(T);
            for (int i = 0; i < numTitles; ++i)
                fields[i] = type.GetField(titles[i]);

            for (int i = 1; i < numLines; ++i)
            {
                string line = lines[i];
                string[] values = line.Split(VALUE_SEPERATOR);
                Log.Assert(values.Length == numTitles);

                // 第一列字段为#的是注释行，跳过。
                if (values[0] == "#")
                    continue;

                T obj = new T();
                for (int j = 0; j < numTitles; ++j)
                {
                    FieldInfo field = fields[j];
                    if (field == null)
                        continue;

                    string strVal = values[j];
                    if (String.IsNullOrEmpty(strVal))
                        continue;

                    Type fieldType = field.FieldType;
                    try
                    {
                        // 按照频率排序
                        if (fieldType == typeof(int))
                            field.SetValue(obj, int.Parse(strVal));
                        else if (fieldType == typeof(string))
                            field.SetValue(obj, strVal);
                        else if (fieldType == typeof(bool))
                            field.SetValue(obj, bool.Parse(strVal));
                        else if (fieldType == typeof(float))
                            field.SetValue(obj, float.Parse(strVal));
                        else if (fieldType.IsSubclassOf(typeof(Enum)))
                            field.SetValue(obj, Enum.Parse(fieldType, strVal));
                        else if (fieldType == typeof(uint))
                            field.SetValue(obj, uint.Parse(strVal));
                        else if (fieldType == typeof(double))
                            field.SetValue(obj, double.Parse(strVal));
                        else // for long,short,byte...
                            field.SetValue(obj, int.Parse(strVal));
                    }
                    catch (Exception)
                    {
                        Log.Assert(false, "解析tab表错误! url:", Url, "line:", i, "field:", field.Name, "value:", strVal);
                    }
                }

                string key = obj.GetKey();
                if (datas.ContainsKey(key))
                {
                    Log.Error("tab表键值重复！ url:", Url, "key:", key);
                }
                else
                {
                    datas[key] = obj;
                    obj.OnComplete();
                }
            }

            parsed = true;
            rawData = null;
        }
    }
}
