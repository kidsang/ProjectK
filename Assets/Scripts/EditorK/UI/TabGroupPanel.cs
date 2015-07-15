using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EditorK.UI
{
    public class TabGroupPanel : MonoBehaviour
    {
        public Transform TabGroup;
        public Transform TabContentGroup;

        private int selectedIndex = -1;
        private Image selectedTab;
        private GameObject selectedGroup;

        private void Start()
        {
            int childCount = TabContentGroup.childCount;
            for (int i = 0; i < childCount; ++i)
                TabContentGroup.GetChild(i).gameObject.SetActive(false);

            SwitchTo(0);
        }

        private void SwitchTo(int index)
        {
            if (index == selectedIndex)
                return;

            if (selectedTab != null)
            {
                selectedTab.color = Color.white;
                selectedGroup.SetActive(false);
            }

            selectedTab = TabGroup.GetChild(index).gameObject.GetComponent<Image>();
            selectedTab.color = EditorUtils.SelectedColor;
            selectedGroup = TabContentGroup.GetChild(index).gameObject;
            selectedGroup.SetActive(true);
        }

        public void OnMouseClick()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            int childCount = TabGroup.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                if (TabGroup.GetChild(i).gameObject == selected)
                {
                    SwitchTo(i);
                    break;
                }
            }
        }

    }
}
