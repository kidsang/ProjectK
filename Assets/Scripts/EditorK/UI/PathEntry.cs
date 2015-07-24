using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;
using ProjectK.Base;

namespace EditorK.UI
{
    public class PathEntry : MonoBehaviour
    {
        public Image ColorField;
        public Text StartField;
        public Text EndField;

        private int index;

        public delegate void Operate(object data);
        private Operate operateStart;
        private Operate operateEnd;

        public void Load(int index, MapPathSetting data, Operate operateStart, Operate operateEnd)
        {
            this.index = index;
            this.operateStart = operateStart;
            this.operateEnd = operateEnd;

            // TODO:
            //ColorField.color = new Color(data.ColorR, data.ColorG, data.ColorB);
            //StartField.text = "起点：(" + data.StartX + ", " + data.StartY + ")";
            //EndField.text = "终点：(" + data.EndX + ", " + data.EndY + ")";
        }

        public void onChangeStart()
        {
            operateStart(index);
        }

        public void OnChangeEnd()
        {
            operateEnd(index);
        }

        private void OnMoveUp()
        {

        }

        private void OnMoveDown()
        {

        }

        private void OnRemove()
        {

        }
    }
}
