using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EditorK.UI
{
    [ExecuteInEditMode]
    public class UILine : MonoBehaviour
    {
        RectTransform line;
        Image image;

        private Vector3 startPoint = new Vector2();
        private Vector3 endPoint = new Vector2(100, 0);
        private float stroke = 1;
        private bool dirty;

        void Start()
        {
            line = gameObject.transform as RectTransform;
            image = gameObject.GetComponent<Image>();
            ApplyTransform();
        }

        public Vector3 StartPoint
        {
            get { return startPoint; }
            set 
            {
                if (startPoint != value)
                {
                    startPoint = value;
                    dirty = true;
#if UNITY_EDITOR
                    ApplyTransform();
#endif
                }
            }
        }

        public Vector3 EndPoint
        {
            get { return endPoint; }
            set 
            {
                if (endPoint != value)
                {
                    endPoint = value;
                    dirty = true;
#if UNITY_EDITOR
                    ApplyTransform();
#endif
                }
            }
        }

        public float Stroke
        {
            get { return stroke; }
            set
            {
                if (stroke != value)
                {
                    stroke = value;
                    dirty = true;
#if UNITY_EDITOR
                    ApplyTransform();
#endif
                }
            }
        }

        public Color LineColor
        {
            get { return image.color; }
            set { image.color = value; }
        }

        void Update()
        {
            if (dirty)
                ApplyTransform();
        }

        private void ApplyTransform()
        {
            Vector2 dir = endPoint - startPoint;
            line.localScale = new Vector3(dir.magnitude, stroke, 0);

            dir.Normalize();
            line.localRotation = Quaternion.FromToRotation(Vector3.right, dir);

            line.localPosition = startPoint;

            dirty = false;
        }
    }
}
