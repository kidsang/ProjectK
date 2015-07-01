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
    public class EditorMouse : DisposableBehaviour
    {
        private static EditorMouse instance;
        public static EditorMouse Instance { get { return instance; } }

        public EditorMap Map;
        public MapCell SelectedMapCell { get; private set; }

        public override void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个EditorMouse实例！");
        }

        void Start()
        {

        }

        private void SelectMapCell()
        {
            DeselectMapCell();

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SelectedMapCell = Map.GetCellByWorldXY(worldPoint);
        }

        private void DeselectMapCell()
        {
            SelectedMapCell = null;
        }

        public void OnSceneMouseClick()
        {
            Log.Info("OnSceneMouseClick");
        }

        public void OnSceneMouseIn()
        {
            Log.Info("OnSceneMouseIn");
        }

        public void OnSceneMouseOut()
        {
            Log.Info("OnSceneMouseOut");
        }
    }
}
