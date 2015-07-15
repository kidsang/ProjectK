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

        // int? index
        MapPathStart,

        // int? index
        MapPathEnd,

        // InfoMap
        //   MapCellFlag flag
        //   int size
        //   bool erase
        TerrainFill,
    }

    public class EditorMouse : DisposableBehaviour
    {
        private static EditorMouse instance;
        public static EditorMouse Instance { get { return instance; } }

        public MapCell SelectedMapCell { get; private set; }
        public Vector2 SelectedLocation { get { return SelectedMapCell == null ? new Vector2() : SelectedMapCell.Location; } }
        public Vector3 SelectedPosition { get { return SelectedMapCell == null ? new Vector3() : SelectedMapCell.Position; } }
        public int SelectedLocationX { get { return SelectedMapCell == null ? 0 : (int)SelectedMapCell.X; } }
        public int SelectedLocationY { get { return SelectedMapCell == null ? 0 : (int)SelectedMapCell.Y; } }
        public float SelectedPositionX { get { return SelectedMapCell == null ? 0 : SelectedMapCell.CenterX; } }
        public float SelectedPositionY { get { return SelectedMapCell == null ? 0 : SelectedMapCell.CenterY; } }

        public MapCell OverMapCell { get { return lastOverMapcell; } }
        public Vector2 OverLocation { get { return OverMapCell == null ? new Vector2() : OverMapCell.Location; } }
        public Vector3 OverPosition { get { return OverMapCell == null ? new Vector3() : OverMapCell.Position; } }
        public int OverLocationX { get { return OverMapCell == null ? 0 : (int)OverMapCell.X; } }
        public int OverLocationY { get { return OverMapCell == null ? 0 : (int)OverMapCell.Y; } }
        public float OverPositionX { get { return OverMapCell == null ? 0 : OverMapCell.CenterX; } }
        public float OverPositionY { get { return OverMapCell == null ? 0 : OverMapCell.CenterY; } }

        public EditorMouseDataType DataType { get; private set; }
        public object Data { get; private set; }
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

        public void SetData(EditorMouseDataType dataType, object data, string previewPath)
        {
            GameObject preview = null;
            if (previewPath != null)
            {
                preview = loader.LoadPrefab(previewPath).Instantiate();
                SpriteRenderer renderer = preview.GetComponent<SpriteRenderer>();
                renderer.color = new Color(1, 1, 1, 0.5f);
            }

            SetData(dataType, data, preview);
        }

        public void SetData(EditorMouseDataType dataType, object data, GameObject preview = null)
        {
            DataType = dataType;
            Data = data;

            if (preview != null)
            {
                if (previewRoot == null)
                    previewRoot = new GameObject("MousePreviewRoot");

                preview.transform.SetParent(previewRoot.transform, false);
                if (lastOverMapcell != null)
                    preview.transform.position = lastOverMapcell.Position;

                this.preview = preview;
            }

            EventManager.Instance.FireEvent(EditorEvent.MOUSE_SET_DATA);
        }

        private void ClearData(bool clearPreview = true)
        {
            if (previewRoot != null && clearPreview)
            {
                Destroy(previewRoot);
                previewRoot = null;
                preview = null;
            }

            if (DataType != EditorMouseDataType.None)
            {
                DataType = EditorMouseDataType.None;
                Data = null;
                EventManager.Instance.FireEvent(EditorEvent.MOUSE_CLEAR_DATA);
            }
        }

        public void Clear(bool clearPreview = true)
        {
            DeselectMapCell();
            ClearData(clearPreview);
        }

        public void OnSceneMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
                EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_DOWN);
            else if (Input.GetMouseButtonDown(1))
                EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_RIGHT_DOWN);
        }

        public void OnSceneMouseUp()
        {
            EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_UP);
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

            //if (lastOverMapcell != null)
            //{
            //    lastOverMapcell.ToWhite();
            //    lastOverMapcell.ShowNeighbours(false);
            //}

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastOverMapcell = map.GetCellByWorldXY(worldPoint);
            if (lastOverMapcell != null)
            {
                //lastOverMapcell.ToBlue();
                //lastOverMapcell.ShowNeighbours(true);

                if (preview != null)
                {
                    preview.transform.position = lastOverMapcell.Position;
                    preview.transform.Translate(0, 0, -10 - preview.transform.localPosition.z);
                }
            }

            lastMousePosition = currentMousePosition;

            EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_OVER_CELL_CHANGE);
        }
    }
}
