using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Maps;
using Assets.Scripts.EditorK.Maps;

namespace Assets.Scripts.EditorK
{
    public enum EditorMouseDataType
    {
        None,
        MapStart,
        MapEnd,
    }

    public class EditorMouse : DisposableBehaviour
    {
        private static EditorMouse instance;
        public static EditorMouse Instance { get { return instance; } }

        public EditorMap Map;
        public MapCell SelectedMapCell { get; private set; }
        public object Data { get; private set; }
        public EditorMouseDataType DataType { get; private set; }

        private ResourceLoader loader;
        private GameObject selectObject;

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

        public void SelectMapCell()
        {
            DeselectMapCell();

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SelectedMapCell = Map.GetCellByWorldXY(worldPoint);

            if (SelectedMapCell != null)
            {
                selectObject.SetActive(true);
                selectObject.transform.position = SelectedMapCell.Position;
            }
        }

        public void DeselectMapCell()
        {
            if (SelectedMapCell != null)
            {
                SelectedMapCell = null;
                selectObject.SetActive(false);
            }
        }

        public void SetData(EditorMouseDataType dataType, object data)
        {
            DeselectMapCell();
            DataType = dataType;
            Data = data;
        }

        public void ClearData()
        {
            DataType = EditorMouseDataType.None;
            Data = null;
        }

        public void Clear()
        {
            DeselectMapCell();
            ClearData();
        }

        public void OnSceneMouseClick()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (DataType == EditorMouseDataType.None)
                    SelectMapCell();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (DataType == EditorMouseDataType.None)
                    DeselectMapCell();
                else
                    ClearData();
            }

            EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_CLICK);
        }

        public void OnSceneMouseIn()
        {
            EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_IN);
        }

        public void OnSceneMouseOut()
        {
            EventManager.Instance.FireEvent(EditorEvent.SCENE_MOUSE_OUT);
        }
    }
}
