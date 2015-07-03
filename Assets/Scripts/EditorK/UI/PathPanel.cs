using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.ProjectK;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Settings;
using Assets.Scripts.EditorK.Maps;
using Assets.Scripts.EditorK.Datas;

namespace Assets.Scripts.EditorK.UI
{
    public class PathPanel : DisposableBehaviour
    {
        private GameObject panelRoot;

        public Transform Content;
        public GameObject EntryPrefab;

        private bool newPath = false;
        private int pathStartX;
        private int pathStartY;
        private int pathEndX;
        private int pathEndY;

        void Start()
        {
            panelRoot = gameObject;

            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_CLICK, OnSceneMouseClick);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnSceneMouseRightClick);
            EventManager.Instance.Register(this, EditorEvent.MAP_ADD_PATH, OnAddPath);
            EventManager.Instance.Register(this, EditorEvent.MAP_REMOVE_PATH, OnRemovePath);
            EventManager.Instance.Register(this, EditorEvent.MAP_SWAP_PATH, OnSwapPath);
        }

        void OnDisable()
        {
            if (newPath)
            {
                newPath = false;
                EditorMouse.Instance.Clear();
            }
        }

        public void OnAddPathButtonClick()
        {
            newPath = true;
            EditorMouse mouse = EditorMouse.Instance;
            mouse.SetData(EditorMouseDataType.MapPathStart, null, "Map/StartMark");
        }

        private void OnSceneMouseClick(object[] args)
        {
            if (!newPath)
                return;

            EditorMouse mouse = EditorMouse.Instance;
            switch (mouse.DataType)
            {
                case EditorMouseDataType.MapPathStart:
                    pathStartX = mouse.SelectedLocationX;
                    pathStartY = mouse.SelectedLocationY;
                    mouse.SetData(EditorMouseDataType.MapPathEnd, null, "Map/EndMark");
                    return;

                case EditorMouseDataType.MapPathEnd:
                    pathEndX = mouse.SelectedLocationX;
                    pathEndY = mouse.SelectedLocationY;
                    mouse.Clear();
                    newPath = false;
                    MapDataProxy.Instance.AddPath(pathStartX, pathStartY, pathEndX, pathEndY);
                    return;
            }
        }

        private void OnSceneMouseRightClick(object[] args)
        {
            if (!newPath)
                return;

            newPath = false;
        }

        private void OnAddPath(object[] args)
        {
            GameObject entryObj = Instantiate(EntryPrefab) as GameObject;
            entryObj.transform.parent = Content;
            entryObj.transform.SetSiblingIndex(Content.childCount - 2);

            //Dictionary<string, object> infos = args[0] as Dictionary<string, object>;
            MapSetting data = MapDataProxy.Instance.Data;
            MapPathSetting pathData = data.Paths[data.Paths.Length - 1];

            PathEntry entry = entryObj.GetComponent<PathEntry>();
            entry.ColorField.color = new Color(pathData.ColorR, pathData.ColorG, pathData.ColorB);
            entry.StartField.text = "起点：(" + pathData.StartX + ", " + pathData.StartY + ")";
            entry.EndField.text = "终点：(" + pathData.EndX + ", " + pathData.EndY + ")";
        }

        private void OnRemovePath(object[] args)
        {

        }

        private void OnSwapPath(object[] args)
        {

        }
    }
}
