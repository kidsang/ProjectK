using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class EditorConfig
    {
        private static EditorConfig instance;
        public static EditorConfig Instance { get { return instance; } }

        private string configPath;
        private Dictionary<string, string> configs = new Dictionary<string,string>();

        public static void Init()
        {
            if (instance == null)
            {
                instance = new EditorConfig();
                instance.configPath = Application.dataPath + "/GameEditorConfig.txt";
                instance.Load();
            }
        }

        private void Load()
        {
            if (File.Exists(configPath))
            {
                char[] sep = new char[] { '=' };
                string[] lines = File.ReadAllLines(configPath);
                foreach (string line in lines)
                {
                    string[] words = line.Split(sep);
                    if (words.Length < 2)
                        continue;

                    string key = words[0].Trim();
                    if (string.IsNullOrEmpty(key))
                        continue;

                    string val = words[1].Trim();
                    if (string.IsNullOrEmpty(val))
                        continue;

                    configs[key] = val;
                }
            }
        }

        private void Save()
        {
            List<string> list = new List<string>();
            foreach (var pair in configs)
            {
                list.Add(pair.Key + "=" + pair.Value);
            }
            string[] lines = list.ToArray();
            File.WriteAllLines(configPath, lines, Encoding.UTF8);
        }

        public string GetValue(string key)
        {
            string val;
            configs.TryGetValue(key, out val);
            return val;
        }

        public void SetValue(string key, object val)
        {
            configs[key] = val.ToString();
            Save();
        }

        public string GetString(string key, string defaultVal = null)
        {
            string str = GetValue(key);
            if (string.IsNullOrEmpty(str))
                str = defaultVal;
            return str;
        }

        public float GetFloat(string key, float defaultVal = 0)
        {
            float val = defaultVal;
            string str = GetValue(key);
            if (!string.IsNullOrEmpty(str))
                float.TryParse(str, out val);
            return val;
        }

        public int GetInt(string key, int defaultVal = 0)
        {
            int val = defaultVal;
            string str = GetValue(key);
            if (!string.IsNullOrEmpty(str))
                int.TryParse(str, out val);
            return val;
        }

        public string LastOpenFilePath
        {
            get { return GetString("LastOpenFilePath", Application.dataPath); }
            set { SetValue("LastOpenFilePath", value); }
        }

        public string LastSaveFilePath
        {
            get { return GetString("LastSaveFilePath", Application.dataPath); }
            set { SetValue("LastSaveFilePath", value); }
        }

        public float CameraZoom
        {
            get { return GetFloat("CameraZoom", 0.5f); }
            set { SetValue("CameraZoom", value); }
        }

        public int TerrainBrushSize
        {
            get { return GetInt("TerrainBrushSize", 1); }
            set { SetValue("TerrainBrushSize", value); }
        }

        public int TerrainVisibleFlags
        {
            get { return GetInt("TerrainVisibleFlags", 0); }
            set { SetValue("TerrainVisibleFlags", value); }
        }
    }
}
