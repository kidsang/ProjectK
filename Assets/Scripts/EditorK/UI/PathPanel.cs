using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK.UI
{
    public class PathPanel : DisposableBehaviour
    {
        public Transform Content;
        public GameObject EntryPrefab;
        public Image AddPathButton;

        private bool initialized = false;
        private bool operating = false;
        private int pathStartX;
        private int pathStartY;
        private int pathEndX;
        private int pathEndY;

        void Start()
        {
            EventManager.Instance.Register(this, EditorEvent.MAP_LOAD, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATHS, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATH, OnUpdatePath);

            if (!initialized)
            {
                initialized = true;
                OnUpdatePaths(null);
            }
        }

        void OnEnable()
        {
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_CLICK, OnSceneMouseClick);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnSceneMouseRightClick);
        }

        void OnDisable()
        {
            EventManager.Instance.Unregister(this, EditorEvent.SCENE_MOUSE_CLICK, OnSceneMouseClick);
            EventManager.Instance.Unregister(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnSceneMouseRightClick);

            if (operating)
                StopOperate();
        }

        public void OnAddPathButtonClick()
        {
            OperateMapPathStart(null);
        }

        private void OnSceneMouseClick(object[] args)
        {
            if (!operating)
                return;

            EditorMouse mouse = EditorMouse.Instance;
            switch (mouse.DataType)
            {
                case EditorMouseDataType.MapPathStart:
                    pathStartX = mouse.SelectedLocationX;
                    pathStartY = mouse.SelectedLocationY;
                    if (mouse.Data == null)
                    {
                        OperateMapPathEnd(null);
                    }
                    else
                    {
                        int index = (int)mouse.Data;
                        MapDataProxy.Instance.SetPathStart(index, pathStartX, pathStartY);
                        StopOperate();
                    }
                    return;

                case EditorMouseDataType.MapPathEnd:
                    pathEndX = mouse.SelectedLocationX;
                    pathEndY = mouse.SelectedLocationY;
                    if (mouse.Data == null)
                    {
                        MapDataProxy.Instance.AddPath(pathStartX, pathStartY, pathEndX, pathEndY);
                    }
                    else
                    {
                        int index = (int)mouse.Data;
                        MapDataProxy.Instance.SetPathEnd(index, pathEndX, pathEndY);
                    }
                    StopOperate();
                    return;
            }
        }

        private void OnSceneMouseRightClick(object[] args)
        {
            if (operating)
                StopOperate();
        }

        private void OperateMapPathStart(object data)
        {
            operating = true;
            AddPathButton.color = EditorUtils.SelectedColor;
            EditorMouse mouse = EditorMouse.Instance;
            mouse.SetData(EditorMouseDataType.MapPathStart, data, "Map/StartMark");
        }

        private void OperateMapPathEnd(object data)
        {
            operating = true;
            AddPathButton.color = EditorUtils.SelectedColor;
            EditorMouse mouse = EditorMouse.Instance;
            mouse.SetData(EditorMouseDataType.MapPathEnd, data, "Map/EndMark");
        }

        private void StopOperate()
        {
            operating = false;
            AddPathButton.color = Color.white;
            EditorMouse mouse = EditorMouse.Instance;
            mouse.Clear();
        }

        private void OnUpdatePaths(object[] args)
        {
            MapSetting data = MapDataProxy.Instance.Data;
            int numPathDatas = data.Paths.Length;
            int numEntryObjs = Content.childCount - 1;

            for (int i = 0; i < numPathDatas; ++i)
            {
                GameObject entryObj;
                if (i < numEntryObjs)
                {
                    entryObj = Content.GetChild(i).gameObject;
                }
                else
                {
                    entryObj = Instantiate(EntryPrefab) as GameObject;
                    entryObj.transform.SetParent(Content);
                    entryObj.transform.SetSiblingIndex(Content.childCount - 2);
                }

                PathEntry entry = entryObj.GetComponent<PathEntry>();
                MapPathSetting pathData = data.Paths[i];
                entry.Load(i, pathData, OperateMapPathStart, OperateMapPathEnd);
            }

            for (int i = numPathDatas; i < numEntryObjs; ++i)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
        }

        private void OnUpdatePath(object[] args)
        {
            var infos = EditorUtils.GetEventInfos(args);
            int index = (int)infos["index"];

            MapSetting data = MapDataProxy.Instance.Data;
            GameObject entryObj = Content.GetChild(index).gameObject;
            PathEntry entry = entryObj.GetComponent<PathEntry>();
            entry.Load(index, data.Paths[index], OperateMapPathStart, OperateMapPathEnd);
        }
    }
}
