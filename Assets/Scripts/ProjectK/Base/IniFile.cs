using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ProjectK.Base
{
    public class IniFile : TextResource
    {
        private static char[] LINE_SEPERATOR = new char[] { '\n', '\r' };

        private bool parsed = false;
        private string rawData;
        private Dictionary<string, Dictionary<string, string>> datas = new Dictionary<string, Dictionary<string, string>>();

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

        public Dictionary<string, string> GetSection(string section)
        {
            if (!parsed)
                Parse();

            if (datas.ContainsKey(section))
                return datas[section];
            return null;
        }

        public string GetString(string section, string key)
        {
            Dictionary<string, string> sectionData = GetSection(section);
            if (sectionData == null)
                return null;

            if (sectionData.ContainsKey(key))
                return sectionData[key];
            return null;
        }

        public int GetInt(string section, string key)
        {
            string str = GetString(section, key);
            int val;
            int.TryParse(str, out val);
            return val;
        }

        public uint GetUInt(string section, string key)
        {
            string str = GetString(section, key);
            uint val;
            uint.TryParse(str, out val);
            return val;
        }

        public float GetFloat(string section, string key)
        {
            string str = GetString(section, key);
            float val;
            float.TryParse(str, out val);
            return val;
        }

        public double GetDouble(string section, string key)
        {
            string str = GetString(section, key);
            double val;
            double.TryParse(str, out val);
            return val;
        }

        public bool GetBool(string section, string key)
        {
            string str = GetString(section, key);
            bool val;
            bool.TryParse(str, out val);
            return val;
        }

        public T GetEnum<T>(string section, string key) where T: struct, IConvertible
        {
            Type enumType = typeof(T);
            Log.Assert(enumType.IsEnum);

            string str = GetString(section, key);
            if (string.IsNullOrEmpty(str))
                return default(T);

            return (T)Enum.Parse(enumType, str);
        }

        private void Parse()
        {
            if (rawData == null)
                return;

            string[] lines = rawData.Split(LINE_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
            int numLines = lines.Length;

            string section = null;
            Dictionary<string, string> sectionData = null;
            for (int i = 0; i < numLines; ++i)
            {
                string line = lines[i];
                line.Trim();

                int commentIndex = line.IndexOf('#');
                if (commentIndex >= 0)
                    line = line.Substring(0, commentIndex);

                if (line.Length == 0)
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    section = line.Substring(1, line.Length - 2);
                    if (datas.ContainsKey(section))
                    {
                        Log.Error("ini表解析错误！Section重复！ url:", Url, "section:", section);
                    }
                    else
                    {
                        sectionData = new Dictionary<string, string>();
                        datas[section] = sectionData;
                    }
                    continue;
                }

                if (sectionData == null)
                {
                    Log.Error("ini表解析错误！缺少初始Section，该行被跳过。 url:", Url, "line:", i, line);
                    continue;
                }

                int equalIndex = line.IndexOf('=');
                if (equalIndex <= 0)
                {
                    Log.Error("ini表解析错误！行格式不正确。 url:", Url, "line:", i, line);
                    continue;
                }

                string key = line.Substring(0, equalIndex);
                string val = line.Substring(equalIndex + 1);
                if (sectionData.ContainsKey(key))
                {
                    Log.Error("ini表解析错误！键值重复！ url:", Url, "section:", section, "key:", key);
                }
                else
                {
                    sectionData[key] = val;
                }
            }

            parsed = true;
            rawData = null;
        }
    }
}
