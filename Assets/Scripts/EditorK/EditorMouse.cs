using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public enum EditorMouseDataType
    {
        None,
        MapPathStart,
        MapPathEnd,
    }

    public class EditorMouse : DisposableBehaviour
    {
        private static EditorMouse instance;
        public static EditorMouse Instance { get { return instance; } }

        public MapCell SelectedMapCell { get; private set; }
        public Vector2 SelectedLocation { get { return SelectedMapCell == null ? new Vector2() : SelectedMapCell.Location; } }
        public Vector2 SelectedPosition { get { return SelectedMapCell == null ? new Vector2() : SelectedMapCell.Position; } }
        public short SelectedLocationX { get { return SelectedMapCell == null ? (short)0 : SelectedMapCell.X; } }
        public short SelectedLocationY { get { return SelectedMapCell == null ? (short)0 : SelectedMapCell.Y; } }
        public float SelectedPositionX { get { return SelectedMapCell == null ? 0 : SelectedMapCell.CenterX; } }
        public float SelectedPositionY { get { return SelectedMapCell == null ? 0 : SelectedMapCell.CenterY; } }

        public EditorMouseDataType DataType { get; private set; }
        public object Data { get; private set; }
        public string PreviewPath { get; private set; }
        private GameObject preview;
        private GameObject previewRoot;

        private ResourceLoader loader;
        private GameObject selectObject;

        private bool mouseIn = false;
        private Vector3 lastMousePosition;
        private MapCell lastOverMapcell;

        public override void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个EditorMouse实例！");
        }

        void Start()
        {
            loader = new ResourceLoader();
            selectObject = loader.LoadPrefab("Map/MapCellSelect").Instantiate();
            selectObject.SetActive(false);
        }

        private void SelectMapCell()
        {
            EditorMap map = GameEditor.Instance.Map;
            if (!map)
                return;

            DeselectMapCell();

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SelectedMapCell = map.GetCellByWorldXY(worldPoint);

            if (SelectedMapCell != null)
            {
                selectObject.SetActive(true);
                selectObject.transform.position = SelectedMapCell.Position;
            }
        }

        private void DeselectMapCell()
        {
            if (SelectedMapCell != null)
            {
                SelectedMapCell = null;
                selectObject.SetActive(false);
            }
        }

        public void SetData(EditorMouseDataType dataType, object data, string previewPath = null)
        {
            DataType = dataType;
            Data = data;
            if (previewPath != null && previewPath != PreviewPath)
            {
                if (previewRoot == null)
                    previewRoot = new GameObject("MousePreviewRoot");

                PreviewPath = previewPath;
                preview = loader.LoadPrefab(previewPath).Instantiate();
                preview.transform.SetParent(previewRoot.transform, false);
                SpriteRenderer renderer = preview.GetComponent<SpriteRenderer>();
                renderer.color = new Color(1, 1, 1, 0.5f);
            }
        }

        private void ClearData(bool clearPreview = true)
        {
            DataType = EditorMouseDataType.None;
            Data = null;

            if (previewRoot != null && clearPreview)
            {
                Destroy(previewRoot);
                previewRoot = null;
                preview = null;
                PreviewPath = null;
            }
        }

        public void Clear(bool clearPreview = true)
        {
            DeselectMapCell();
            ClearData(clearPreview);
        }

        public void OnSceneMouseClick()
        {
            if (Input.GetMouseButtonUp(0))
            {
                SelectMapCell();
                EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_CLICK);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Clear();
                EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_RIGHT_CLICK);
            }
        }

        public void OnSceneMouseIn()
        {
            mouseIn = true;
            EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_IN);
        }

        public void OnSceneMouseOut()
        {
            mouseIn = false;
            EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_OUT);
        }

        void Update()
        {
            if (!mouseIn)
                return;

            EditorMap map = GameEditor.Instance.Map;
            if (!map)
                return;

            Vector3 currentMousePosition = Input.mousePosition;
            if (currentMousePosition == lastMousePosition)
                return;

            if (lastOverMapcell != null)
            {
                lastOverMapcell.ToWhite();
                lastOverMapcell.ShowNeighbours(false);
            }

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastOverMapcell = map.GetCellByWorldXY(worldPoint);
            if (lastOverMapcell != null)
            {
                lastOverMapcell.ToBlue();
                lastOverMapcell.ShowNeighbours(true);

                if (preview != null)
                    preview.transform.position = lastOverMapcell.Position;
            }

            lastMousePosition = currentMousePosition;
        }
    }
}
