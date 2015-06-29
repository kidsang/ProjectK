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
        }
    }
}
