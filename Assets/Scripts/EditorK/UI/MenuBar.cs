using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class MenuBar : MonoBehaviour
    {
        private bool menuOpen = false;
        private bool menuClick = false;
        private GameObject currentMenu;

        public void OnMenuMouseClick(GameObject menu)
        {
            if (currentMenu == menu)
                return;

            menuClick = true;
            menuOpen = true;
            HideMenu(currentMenu);
            ShowMenu(menu);
            currentMenu = menu;
        }

        public void OnMenuMouseOver(GameObject menu)
        {
            if (!menuOpen || currentMenu == menu)
                return;

            HideMenu(currentMenu);
            ShowMenu(menu);
            currentMenu = menu;
        }

        private void ShowMenu(GameObject menu)
        {
            if (menu == null)
                return;

            menu.GetComponent<Button>().Select();
            menu.transform.GetChild(1).gameObject.SetActive(true);
        }

        private void HideMenu(GameObject menu)
        {
            if (menu == null)
                return;

            menu.transform.GetChild(1).gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!menuClick && currentMenu != null)
            {
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
                {
                    HideMenu(currentMenu);
                    currentMenu = null;
                    menuOpen = false;
                }
            }

            menuClick = false;
        }

        public void OnUndo()
        {
            MapDataProxy.Instance.Undo();
        }

        public void OnRedo()
        {
            MapDataProxy.Instance.Redo();
        }
    }
}
