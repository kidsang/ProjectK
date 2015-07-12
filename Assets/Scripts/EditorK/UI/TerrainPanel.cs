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

        public override void Awake()
        {
            base.Awake();

            for (int i = Content.childCount - 1; i > 0; --i)
                DestroyImmediate(Content.GetChild(i).gameObject);

            TerrainFlagInfo[] infos = TerrainFlagInfo.Infos;
            foreach (TerrainFlagInfo info in infos)
            {
                GameObject entryObj = Instantiate(EntryPrefab) as GameObject;
                entryObj.transform.SetParent(Content, false);

                TerrainEntry entry = entryObj.GetComponent<TerrainEntry>();
                entry.Load(info);

                entryObj.GetComponent<Button>().onClick.AddListener(OnMouseClick);
            }
        }

        private void OnEnable()
        {
            SelectEntry(0);
            EventManager.Instance.Register(this, EditorEvent.SCENE_MOUSE_RIGHT_CLICK, OnMouseRightClick);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterAll(this);
            DeselectEntry();
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
        }

        private void DeselectEntry()
        {
            if (!selectedEntry)
                return;

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
                    {
                        DeselectEntry();
                        EditorMouse.Instance.Clear();
                    }
                    else
                    {
                        SelectEntry(i - 1);
                    }
                }
            }
        }

        private void OnMouseRightClick(object[] args)
        {
            DeselectEntry();
        }
    }
}
