using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.EditorK.Maps;

namespace Assets.Scripts.EditorK.UI
{
    public class ScenePanel : DisposableBehaviour
    {
        public Scrollbar HScrollBar;
        public Scrollbar VScrollBar;
        public RectTransform CanvasArea;
        public RectTransform ViewArea;

        private int lastScreenWidth;
        private int lastScreenHeight;
        private bool resizing;

        void Start()
        {
        }

        void Update()
        {
            if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                Screen.SetResolution(lastScreenWidth, lastScreenHeight, false);

                OnScreenResize();
            }
        }

        private void OnScreenResize()
        {
            resizing = true;

            Camera camera = Camera.main;
            EditorMap map = Editor.Instance.Map;
            Rect viewRect = ViewArea.rect;
            Rect canvasRect = CanvasArea.rect;

            // 把摄像机视图限制在ViewArea中
            Vector3 viewPosition = ViewArea.position;
            Rect cameraRect = new Rect(viewPosition.x / canvasRect.width, viewPosition.y / canvasRect.height,
                viewRect.width / canvasRect.width, viewRect.height / canvasRect.height);
            camera.rect = cameraRect;
            // 同时还要保持MapCell的原始尺寸不被拉伸
            camera.orthographicSize = lastScreenHeight / 200.0f * cameraRect.height;

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

            HScrollBar.size = Mathf.Min(1, viewRect.width / (map.Width * 100.0f));
            HScrollBar.value = Mathf.Min(1, (cameraPosition.x - left) / (right - left));
            VScrollBar.size = Mathf.Min(1, viewRect.height / (map.Height * 100.0f));
            VScrollBar.value = Mathf.Min(1, (cameraPosition.y - bottom) / (top - bottom));

            resizing = false;
        }

        public void OnHScrollBarValueChange()
        {
            if (resizing)
                return;

            Camera camera = Camera.main;
            EditorMap map = Editor.Instance.Map;
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
            EditorMap map = Editor.Instance.Map;
            float cameraHalfHeight = camera.orthographicSize;
            float bottom = cameraHalfHeight;
            float top = map.Height - cameraHalfHeight;

            Vector3 cameraPosition = camera.transform.position;
            cameraPosition.y = bottom + (top - bottom) * VScrollBar.value;
            camera.transform.position = cameraPosition;
        }
    }
}
