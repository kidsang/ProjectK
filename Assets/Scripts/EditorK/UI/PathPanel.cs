using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.ProjectK;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.EditorK.Maps;
using Assets.Scripts.EditorK.Datas;

namespace Assets.Scripts.EditorK.UI
{
    public class PathPanel : DisposableBehaviour
    {
        private GameObject panelObject;

        private bool newPath = false;
        private int pathStartX;
        private int pathStartY;
        private int pathEndX;
        private int pathEndY;

        void Start()
        {
            panelObject = gameObject;

            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_CLICK, OnSceneMouseClick);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnSceneMouseRightClick);
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
    }
}
