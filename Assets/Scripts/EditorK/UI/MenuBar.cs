using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class MenuBar : MonoBehaviour
    {
        private static MenuBar instance;
        public static MenuBar Instance { get { return instance; } }

        private bool menuOpen = false;
        private bool menuClick = false;
        private GameObject currentMenu;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个MenuBar实例！");
        }

        public void OnMenuMouseClick(GameObject menu)
        {
            if (currentMenu == menu)
                return;

            menuClick = true;
            menuOpen = true;
            HideMenu(currentMenu);
            ShowMenu(menu);
            currentMenu = menu;
        }

        public void OnMenuMouseOver(GameObject menu)
        {
            if (!menuOpen || currentMenu == menu)
                return;

            HideMenu(currentMenu);
            ShowMenu(menu);
            currentMenu = menu;
        }

        private void ShowMenu(GameObject menu)
        {
            if (menu == null)
                return;

            menu.GetComponent<Button>().Select();
            menu.transform.GetChild(1).gameObject.SetActive(true);
        }

        private void HideMenu(GameObject menu)
        {
            if (menu == null)
                return;

            menu.transform.GetChild(1).gameObject.SetActive(false);
        }

        private void CloseMenu()
        {
            HideMenu(currentMenu);
            currentMenu = null;
            menuOpen = false;
            menuClick = false;
        }

        private void LateUpdate()
        {
            if (!menuClick && currentMenu != null)
            {
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
                {
                    CloseMenu();
                }
            }

            menuClick = false;
        }

        public void NewFile()
        {
            GameEditor.Instance.NewMap();
        }

        public void OpenFile()
        {
            string path = EditorConfig.Instance.LastOpenFilePath;
            OpenFileName ofn = new OpenFileName("打开文件", path, false);
            if (GetOpenFileName(ofn))
            {
                path = ofn.file;
                EditorConfig.Instance.LastOpenFilePath = path;

                string json = File.ReadAllText(path);
                MapSetting data = SimpleJson.DeserializeObject<MapSetting>(json);
                GameEditor.Instance.LoadMap(data, path);
            }

            CloseMenu();
        }

        public void SaveFile()
        {
            string path = GameEditor.Instance.FilePath;
            if (path == null)
                SaveFileAs();
            else
                DoSaveFile(path);
        }

        public void SaveFileAs()
        {
            string path = EditorConfig.Instance.LastSaveFilePath;
            OpenFileName ofn = new OpenFileName("保存文件", path, true);
            if (GetSaveFileName(ofn))
            {
                path = ofn.file;
                EditorConfig.Instance.LastSaveFilePath = path;
                DoSaveFile(path);
            }

            CloseMenu();
        }

        private void DoSaveFile(string path)
        {
            MapSetting data = MapDataProxy.Instance.Data;
            string json = SimpleJson.SerializeObject(data);
            File.WriteAllText(path, json, Encoding.UTF8);

            GameEditor.Instance.FilePath = path;
            GameEditor.Instance.FileModified = false;
        }

        public void Undo()
        {
            MapDataProxy.Instance.Undo();
        }

        public void Redo()
        {
            MapDataProxy.Instance.Redo();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OpenFileName
        {
            public int structSize = 0;
            public IntPtr dlgOwner = IntPtr.Zero;
            public IntPtr instance = IntPtr.Zero;
            public String filter = null;
            public String customFilter = null;
            public int maxCustFilter = 0;
            public int filterIndex = 0;
            public String file = null;
            public int maxFile = 0;
            public String fileTitle = null;
            public int maxFileTitle = 0;
            public String initialDir = null;
            public String title = null;
            public int flags = 0;
            public short fileOffset = 0;
            public short fileExtension = 0;
            public String defExt = null;
            public IntPtr custData = IntPtr.Zero;
            public IntPtr hook = IntPtr.Zero;
            public String templateName = null;
            public IntPtr reservedPtr = IntPtr.Zero;
            public int reservedInt = 0;
            public int flagsEx = 0;

            private static int OFN_FILEMUSTEXIST = 0x00001000;
            private static int OFN_NOCHANGEDIR = 0x00000008;

            public OpenFileName(string titleText, string initPath, bool forSave)
            {
                structSize = Marshal.SizeOf(this);
                dlgOwner = GameEditor.Instance.GetWindowPtr();
                filter = "*.map\0*.map\0";
                file = new string(new char[256]);
                maxFile = file.Length;
                fileTitle = new string(new char[64]);
                maxFileTitle = fileTitle.Length;
                initialDir = Path.GetFullPath(initPath);
                title = titleText;
                defExt = "map";
                flags = OFN_NOCHANGEDIR | OFN_FILEMUSTEXIST;
            }
        }

        [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    }
}
