﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class EditorKeyboard : DisposableBehaviour
    {
        private static EditorKeyboard instance;
        public static EditorKeyboard Instance { get { return instance; } }

        private HashSet<KeyCode> downKeys = new HashSet<KeyCode>();

        public override void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new Exception("多个EditorKeyboard实例！");
        }

        private void OnGUI()
        {
            Event evt = Event.current;
            if (!evt.isKey)
                return;

            KeyCode key = evt.keyCode;
            if (evt.type == EventType.KeyDown)
            {
                if (!downKeys.Contains(key))
                {
                    downKeys.Add(key);
                    OnKeyDown(evt);
                }
            }
            else if (evt.type == EventType.KeyUp)
            {
                if (downKeys.Contains(key))
                {
                    downKeys.Remove(key);
                    OnKeyUp(evt);
                }
            }
        }

        private void OnKeyDown(Event evt)
        {
            KeyCode key = evt.keyCode;
            switch (key)
            {
                case KeyCode.Z:
                    if (evt.control)
                        MapDataProxy.Instance.Undo();
                    return;

                case KeyCode.Y:
                    if (evt.control)
                        MapDataProxy.Instance.Redo();
                    return;
            }
        }

        private void OnKeyUp(Event evt)
        {

        }
    }
}
