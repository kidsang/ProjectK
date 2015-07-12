﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;

namespace EditorK
{
    public class TerrainEntry : MonoBehaviour
    {
        public Image Background;
        public Image ColorField;
        public Text NameField;
        public Text FlagField;
        public Toggle VisibleField;

        public void Load(TerrainFlagInfo info)
        {
            ColorField.color = info.Color;
            NameField.text = info.Name;
            FlagField.text = info.FlagName;
        }

        public void Select()
        {
            Color color = ColorField.color;
            color.a = 0.5f;
            Background.color = color;
        }

        public void Deselect()
        {
            Color color = Color.white;
            color.a = 0.5f;
            Background.color = color;
        }
    }
}
