using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.ProjectK;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.EditorK.Maps;

namespace Assets.Scripts.EditorK.UI
{
    public class NavigatePanel : DisposableBehaviour
    {
        public Camera MiniMapCamera;
        public RectTransform CanvasArea;
        public RectTransform ViewArea;
        public RectTransform SceneArea;
        public RectTransform Box;

        private bool draging = false;
        private Vector3 lastMousePosition;

        void Start()
        {
            EventManager.Instance.Register(this, EditorEvent.SCREEN_RESIZE, OnScreenResize);
        }

        private void OnScreenResize(object[] args)
        {
            Camera camera = MiniMapCamera;
            Rect viewRect = ViewArea.rect;
            Rect canvasRect = CanvasArea.rect;

            // 把摄像机视图限制在ViewArea中
            Vector3 viewPosition = ViewArea.position;
            Rect cameraRect = new Rect(viewPosition.x / canvasRect.width, viewPosition.y / canvasRect.height,
                viewRect.width / canvasRect.width, viewRect.height / canvasRect.height);
            camera.rect = cameraRect;

            EditorMap map = GameEditor.Instance.Map;
            camera.transform.position = new Vector3(map.Width / 2, map.Height / 2, -10);

            camera.orthographicSize = map.Height / 2;
            camera.orthographicSize = Mathf.Max(camera.orthographicSize, map.Width / 2 / camera.aspect);

            Rect sceneRect = SceneArea.rect;
            Box.sizeDelta = new Vector2(sceneRect.width / map.Width * viewRect.width * GameDefines.PixelToUnitF, sceneRect.height / map.Height * viewRect.height * GameDefines.PixelToUnitF);
            Vector3 mainCameraPosition = Camera.main.transform.position;
            Box.localPosition = new Vector3(mainCameraPosition.x / map.Width * viewRect.width, mainCameraPosition.y / map.Height * viewRect.height);
        }

        public void OnMouseDown()
        {
            draging = true;
            lastMousePosition = Input.mousePosition;
        }

        void Update()
        {
            if (draging)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    draging = false;
                }
                else
                {
                    Vector3 curMousePosition = Input.mousePosition;
                    Vector3 deltaMousePosition = curMousePosition - lastMousePosition;
                    Box.localPosition += deltaMousePosition;
                    lastMousePosition = curMousePosition;

                    // TODO: 这个几把功能优先级调低
                }
            }
        }
    }
}
