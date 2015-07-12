using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EditorK.UI
{
    using DetailsPanelDict = Dictionary<DetailsPanelType, DetailsPanelBase>;

    public enum DetailsPanelType
    {
        // int index
        Terrain,
    }

    public class DetailsPanels : MonoBehaviour
    {
        private static DetailsPanels instance;
        public static DetailsPanels Instance { get { return instance; } }

        public TerrainDetailsPanel TerrainPanel;

        private DetailsPanelDict panels = new DetailsPanelDict();
        private HashSet<DetailsPanelBase> showingPanels = new HashSet<DetailsPanelBase>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("DetailsPanels！");

            panels[DetailsPanelType.Terrain] = TerrainPanel;
        }

        public void ShowPanel(DetailsPanelType panelType, InfoMap infos)
        {
            DetailsPanelBase panel = panels[panelType];
            if (!showingPanels.Contains(panel))
            {
                showingPanels.Add(panel);
                panel.gameObject.SetActive(true);
            }

            panel.Refresh(infos);
        }

        public void HidePanel(DetailsPanelType panelType)
        {
            DetailsPanelBase panel = panels[panelType];
            if (showingPanels.Contains(panel))
            {
                showingPanels.Remove(panel);
                panel.gameObject.SetActive(false);
            }
        }

        public void Clear()
        {
            foreach (DetailsPanelBase panel in showingPanels)
                panel.gameObject.SetActive(false);
            showingPanels.Clear();
        }
    }
}
