using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EditorK.UI
{
    public class PopupMenu : MonoBehaviour
    {
        private static PopupMenu instance;

        public Transform Content;
        public GameObject MenuItemPrefab;
        public delegate void OnMenuItemClick(object data);

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个PopupMenu实例！");
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
                Hide();
        }

        public static void AddItem(string text, object data, OnMenuItemClick callback, bool enabled = true)
        {
            GameObject menuItemObj = Instantiate(instance.MenuItemPrefab) as GameObject;
            menuItemObj.transform.SetParent(instance.Content);

            PopupMenuItem menuItem = menuItemObj.GetComponent<PopupMenuItem>();
            menuItem.Data = data;
            menuItem.Callback = callback;
            menuItem.SetText(text);
            menuItem.SetEnabled(enabled);
        }

        public static void Show()
        {
            Vector3 position = Input.mousePosition;
            instance.transform.position = position;
            instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            Transform content = instance.Content;
            for (int i = content.childCount - 1; i >= 0; --i)
                Destroy(content.GetChild(i).gameObject);

            instance.gameObject.SetActive(false);
        }
    }
}
