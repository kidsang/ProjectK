using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EditorK.UI
{
    [ExecuteInEditMode]
    public class UIBox : MonoBehaviour
    {
        public Image LeftImage;
        public Image TopImage;
        public Image RightImage;
        public Image BottomImage;

        public Color BoxColor
        {
            get { return TopImage.color; }
            set
            {
                LeftImage.color = value;
                TopImage.color = value;
                RightImage.color = value;
                BottomImage.color = value;
            }
        }
    }
}
