using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectK;
using ProjectK.Base;

namespace EditorK.UI
{
    public class TerrainPanel : DisposableBehaviour
    {
        public Transform Content;
        public GameObject EntryPrefab;

        private TerrainEntry selectedEntry;
        private bool draging = false;

        public override void Awake()
        {
            base.Awake();

            for (int i = Content.childCount - 1; i > 0; --i)
                DestroyImmediate(Content.GetChild(i).gameObject);

            foreach (TerrainFlagInfo info in TerrainFlagInfo.Infos)
            {
                GameObject entryObj = Instantiate(EntryPrefab) as GameObject;
                entryObj.transform.SetParent(Content, false);

                TerrainEntry entry = entryObj.GetComponent<TerrainEntry>();
                entry.Load(info);

                entryObj.GetComponent<Button>().onClick.AddListener(OnMouseClick);
            }
        }

        private void Start()
        {
            int visibleFlags = EditorConfig.Instance.TerrainVisibleFlags;
            for (int i = 1; i < Content.childCount; ++i)
            {
                GameObject entryObj = Content.GetChild(i).gameObject;
                TerrainEntry entry = entryObj.GetComponent<TerrainEntry>();

                bool visible = EditorUtils.HasFlag(visibleFlags, (int)entry.Flag);
                entry.VisibleField.isOn = visible;
                entry.OnVisibleChange = OnTerrainEntryVisibleChange;
            }
        }

        private void OnEnable()
        {
            SelectEntry(0);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_DOWN, OnSceneMouseDown);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_UP, OnSceneMouseUp);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_OVER_CELL_CHANGE, OnSceneMouseOverCellChange);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnMouseRightClick);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterAll(this);
            DeselectEntry();
            OnSceneMouseUp(null);
        }

        private void SelectEntry(int index)
        {
            DeselectEntry();
            GameObject entryObj = Content.GetChild(index + 1).gameObject;
            selectedEntry = entryObj.GetComponent<TerrainEntry>();
            selectedEntry.Select();

            InfoMap infos = new InfoMap();
            infos["index"] = index;
            DetailsPanels.Instance.ShowPanel(DetailsPanelType.Terrain, infos);

            if (GameEditor.Instance.Map != null)
                GameEditor.Instance.Map.ToggleShowTerrain(selectedEntry.Flag, true);
        }

        private void DeselectEntry()
        {
            if (!selectedEntry)
                return;

            if (!selectedEntry.VisibleField.isOn && GameEditor.Instance.Map != null)
                GameEditor.Instance.Map.ToggleShowTerrain(selectedEntry.Flag, false);

            selectedEntry.Deselect();
            selectedEntry = null;
            DetailsPanels.Instance.HidePanel(DetailsPanelType.Terrain);
        }

        public void OnMouseClick()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            int childCount = Content.childCount;
            for (int i = 1; i < childCount; ++i)
            {
                GameObject entryObj = Content.GetChild(i).gameObject;
                if (entryObj == selected)
                {
                    TerrainEntry entry = entryObj.GetComponent<TerrainEntry>();
                    if (entry == selectedEntry)
                        DeselectEntry();
                    else
                        SelectEntry(i - 1);
                }
            }
        }

        private void OnMouseRightClick(object[] args)
        {
            DeselectEntry();
        }

        private void OnSceneMouseDown(object[] args)
        {
            EditorMouse mouse = EditorMouse.Instance;
            if (mouse.DataType != EditorMouseDataType.TerrainFill)
                return;

            draging = true;
            MapDataProxy.Instance.Recording = false;
            OnSceneMouseOverCellChange(null);
        }

        private void OnSceneMouseUp(object[] args)
        {
            if (!draging)
                return;

            MapDataProxy.Instance.Recording = true;
            OnSceneMouseOverCellChange(null);
            draging = false;
        }

        private void OnSceneMouseOverCellChange(object[] args)
        {
            if (!draging)
                return;

            EditorMouse mouse = EditorMouse.Instance;
            MapCell cell = mouse.OverMapCell;
            if (cell == null)
                return;

            InfoMap infos = mouse.Data as InfoMap;
            MapCellFlag flag = (MapCellFlag)infos["flag"];
            int size = (int)infos["size"];
            bool erase = (bool)infos["erase"];

            MapDataProxy.Instance.SetTerrainFlag(cell.X, cell.Y, size - 1, flag, !erase);
        }

        private void OnTerrainEntryVisibleChange(TerrainEntry entry)
        {
            MapCellFlag flag = entry.Flag;
            bool visible = entry.VisibleField.isOn;
            int visibleFlags = EditorConfig.Instance.TerrainVisibleFlags;
            EditorUtils.SetFlag(ref visibleFlags, (int)flag, visible);
            EditorConfig.Instance.TerrainVisibleFlags = visibleFlags;

            if (entry != selectedEntry)
                GameEditor.Instance.Map.ToggleShowTerrain(flag, visible);
        }

        public void OnToggleAllVisibleButtonClick()
        {
            bool visible = false;
            for (int i = 1; i < Content.childCount; ++i)
                visible = visible || !Content.GetChild(i).gameObject.GetComponent<TerrainEntry>().VisibleField.isOn;

            for (int i = 1; i < Content.childCount; ++i)
                Content.GetChild(i).gameObject.GetComponent<TerrainEntry>().VisibleField.isOn = visible;
        }
    }
}
