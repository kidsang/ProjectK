using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using EditorK.UI;

namespace EditorK.Editor
{
    [CustomEditor(typeof(UIBox))]
    public class UIBoxInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            UIBox box = target as UIBox;

            box.BoxColor = EditorGUILayout.ColorField("Color", box.BoxColor);
            if (GUI.changed)
                EditorUtility.SetDirty(box);
        }
    }
}
