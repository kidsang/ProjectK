using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class ScenePanel : DisposableBehaviour
    {
        public Scrollbar HScrollBar;
        public Scrollbar VScrollBar;
        public RectTransform CanvasArea;
        public RectTransform ViewArea;

        public Transform MapPathRoot;

        private ResourceLoader loader;
        private int lastScreenWidth;
        private int lastScreenHeight;
        private bool resizing;

        void Start()
        {
            loader = new ResourceLoader();

            EventManager.Instance.Register(this, EditorEvent.MAP_LOAD, OnMapLoad);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATHS, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATH, OnUpdatePath);
        }

        void Update()
        {
            if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                Screen.SetResolution(lastScreenWidth, lastScreenHeight, false);

                OnScreenResize();
                EventManager.Instance.FireEvent(EditorEvent.SCREEN_RESIZE);
            }
        }

        private void OnScreenResize()
        {
            EditorMap map = GameEditor.Instance.Map;
            if (!map)
                return;

            resizing = true;

            Camera camera = Camera.main;
            Rect viewRect = ViewArea.rect;
            Rect canvasRect = CanvasArea.rect;

            // 把摄像机视图限制在ViewArea中
            Vector3 viewPosition = ViewArea.position;
            Rect cameraRect = new Rect(viewPosition.x / canvasRect.width, viewPosition.y / canvasRect.height,
                viewRect.width / canvasRect.width, viewRect.height / canvasRect.height);
            camera.rect = cameraRect;
            // 同时还要保持MapCell的原始尺寸不被拉伸
            camera.orthographicSize = lastScreenHeight / 2 * GameDefines.PixelToUnitF * cameraRect.height;

            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = cameraHalfHeight * camera.aspect;
            float left = cameraHalfWidth;
            float right = map.Width - cameraHalfWidth;
            float bottom = cameraHalfHeight;
            float top = map.Height - cameraHalfHeight;

            Vector3 cameraPosition = camera.transform.position;
            if (cameraPosition.x < left)
                cameraPosition.x = left;
            else if (cameraPosition.x > right)
                cameraPosition.x = right;
            if (cameraPosition.y < bottom)
                cameraPosition.y = bottom;
            else if (cameraPosition.y > top)
                cameraPosition.y = top;
            camera.transform.position = cameraPosition;

            HScrollBar.size = Mathf.Min(1, viewRect.width / (map.Width * GameDefines.UnitToPixelF));
            HScrollBar.value = Mathf.Min(1, (cameraPosition.x - left) / (right - left));
            VScrollBar.size = Mathf.Min(1, viewRect.height / (map.Height * GameDefines.UnitToPixelF));
            VScrollBar.value = Mathf.Min(1, (cameraPosition.y - bottom) / (top - bottom));

            resizing = false;
        }

        public void OnHScrollBarValueChange()
        {
            if (resizing)
                return;

            Camera camera = Camera.main;
            EditorMap map = GameEditor.Instance.Map;
            float cameraHalfWidth = camera.orthographicSize * camera.aspect;
            float left = cameraHalfWidth;
            float right = map.Width - cameraHalfWidth;

            Vector3 cameraPosition = camera.transform.position;
            cameraPosition.x = left + (right - left) * HScrollBar.value;
            camera.transform.position = cameraPosition;
        }

        public void OnVScrollBarValueChange()
        {
            if (resizing)
                return;

            Camera camera = Camera.main;
            EditorMap map = GameEditor.Instance.Map;
            float cameraHalfHeight = camera.orthographicSize;
            float bottom = cameraHalfHeight;
            float top = map.Height - cameraHalfHeight;

            Vector3 cameraPosition = camera.transform.position;
            cameraPosition.y = bottom + (top - bottom) * VScrollBar.value;
            camera.transform.position = cameraPosition;
        }

        private void OnMapLoad(object[] args)
        {
            OnScreenResize();
            OnUpdatePaths(null);
        }

        private void OnUpdatePaths(object[] args)
        {
            MapSetting data = MapDataProxy.Instance.Data;
            int numPathDatas = data.Paths.Length;
            int numPathObjs = MapPathRoot.childCount;

            for (int i = 0; i < numPathDatas; ++i)
            {
                GameObject pathObj;
                GameObject startObj;
                GameObject endObj;
                if (i < numPathObjs)
                {
                    pathObj = MapPathRoot.GetChild(i).gameObject;
                    startObj = pathObj.transform.GetChild(0).gameObject;
                    endObj = pathObj.transform.GetChild(1).gameObject;
                }
                else
                {
                    pathObj = new GameObject("Path" + i);
                    pathObj.transform.SetParent(MapPathRoot);
                    startObj = loader.LoadPrefab("Map/StartMark").Instantiate();
                    startObj.transform.SetParent(pathObj.transform);
                    endObj = loader.LoadPrefab("Map/EndMark").Instantiate();
                    endObj.transform.SetParent(pathObj.transform);
                }

                MapPathSetting pathData = data.Paths[i];
                SetPath(pathData, startObj, endObj);
            }

            for (int i = numPathDatas; i < numPathObjs; ++i)
            {
                Destroy(MapPathRoot.GetChild(i).gameObject);
            }
        }

        private void OnUpdatePath(object[] args)
        {
            var infos = EditorUtils.GetEventInfos(args);
            int index = (int)infos["index"];

            MapSetting data = MapDataProxy.Instance.Data;
            MapPathSetting pathData = data.Paths[index];
            GameObject pathObj = MapPathRoot.GetChild(index).gameObject;
            GameObject startObj = pathObj.transform.GetChild(0).gameObject;
            GameObject endObj = pathObj.transform.GetChild(1).gameObject;
            SetPath(pathData, startObj, endObj);
        }

        private void SetPath(MapPathSetting pathData, GameObject startObj, GameObject endObj)
        {
            EditorMap map = GameEditor.Instance.Map;
            startObj.transform.localPosition = map.GetCell(pathData.StartX, pathData.StartY).Position;
            endObj.transform.localPosition = map.GetCell(pathData.EndX, pathData.EndY).Position;
            Color color = new Color(pathData.ColorR, pathData.ColorG, pathData.ColorB);
            startObj.GetComponent<SpriteRenderer>().color = color;
            endObj.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
