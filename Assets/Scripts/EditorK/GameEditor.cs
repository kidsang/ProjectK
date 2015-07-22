using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class GameEditor : DisposableBehaviour
    {
        private static GameEditor instance;
        public static GameEditor Instance { get { return instance; } }

        private GameObject sceneRoot;
        public EditorMap Map;
        public Canvas UICanvas;

        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(String className, String windowName);

        private static string baseTitle = "ProjectK";
        private string title = baseTitle;
        private string filePath;
        private bool fileModified;

        public override void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个GameEditor实例！");

            sceneRoot = gameObject;
            EditorConfig.Init();

            ResourceManager.Init();
            SettingManager.Init(StartEditor);
        }

        void Start()
        {
        }

        private void StartEditor()
        {
            NewMap();
        }

        public void NewMap()
        {
            MapSetting data = new MapSetting();
            data.CellCountX = 10;
            data.CellCountY = 10;
            LoadMap(data);
        }

        public void LoadMap(MapSetting data, string path = null)
        {
            if (Map)
                Map.Dispose();

            GameObject mapRoot = new GameObject("MapRoot");
            mapRoot.transform.SetParent(sceneRoot.transform);
            mapRoot.transform.SetSiblingIndex(0);

            Map = mapRoot.AddComponent<EditorMap>();
            Map.New(data);

            MapDataProxy.Instance.Load(data, path);

            fileModified = false;
            filePath = path;
            ChangeWindowTitle();
        }

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (filePath == value)
                    return;
                filePath = value;
                ChangeWindowTitle();
            }
        }

        public bool FileModified
        {
            get { return fileModified; }
            set
            {
                if (fileModified == value)
                    return;
                fileModified = value;
                ChangeWindowTitle();
            }
        }

        public IntPtr GetWindowPtr()
        {
            IntPtr windowPtr = FindWindow(null, title);
            return windowPtr;
        }

        public void ChangeWindowTitle()
        {
            IntPtr windowPtr = GetWindowPtr();
            title = baseTitle + " - ";

            if (filePath == null)
                title += "New Map";
            else
                title += System.IO.Path.GetFileName(filePath);

            if (fileModified)
                title += "*";

            SetWindowText(windowPtr, title);
        }
    }
}
