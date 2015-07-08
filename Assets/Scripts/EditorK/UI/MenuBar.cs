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
        private static MenuBar instance;
        public static MenuBar Instance { get { return instance; } }

        private bool menuOpen = false;
        private bool menuClick = false;
        private GameObject currentMenu;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个MenuBar实例！");
        }

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

        public void NewFile()
        {
            GameEditor.Instance.NewMap();
        }

        public void OpenFile()
        {

        }

        public void SaveFile()
        {

        }

        public void SaveFileAs()
        {

        }

        public void Undo()
        {
            MapDataProxy.Instance.Undo();
        }

        public void Redo()
        {
            MapDataProxy.Instance.Redo();
        }
    }
}
