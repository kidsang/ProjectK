using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EditorK.UI
{
    public class PopupMenuItem : MonoBehaviour
    {
        public Text text;

        public object Data;
        public PopupMenu.OnMenuItemClick Callback;

        public void OnMouseClick()
        {
            Callback(Data);
        }

        public void SetText(string value)
        {
            text.text = value;
        }

        public void SetEnabled(bool value)
        {
            Color c = text.color;
            c.a = value ? 1 : 0.5f;
            text.color = c;
        }
    }
}
