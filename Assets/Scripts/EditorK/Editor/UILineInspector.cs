using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using EditorK.UI;

namespace EditorK.Editor
{
    [CustomEditor(typeof(UILine))]
    public class UILineInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            UILine line = target as UILine;

            Vector2 point = new Vector2(line.StartPoint.x, line.StartPoint.y);
            point = EditorGUILayout.Vector2Field("Start Point", point);
            line.StartPoint = new Vector3(point.x, point.y);

            point = new Vector2(line.EndPoint.x, line.EndPoint.y);
            point = EditorGUILayout.Vector2Field("End Point", point);
            line.EndPoint = new Vector3(point.x, point.y);

            float stroke = EditorGUILayout.FloatField("Stroke", line.Stroke);
            line.Stroke = stroke;
        }
    }
}
