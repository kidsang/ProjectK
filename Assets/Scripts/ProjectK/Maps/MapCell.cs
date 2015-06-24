using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK.Maps
{
    /**
     * MapCell使用Axial坐标系，其中x + y + z = 0，因此只需要x和y的就可以确定一个MapCell。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class MapCell : DisposableBehaviour
    {
        public static float Radius = 1.0f;
        public static float Sqrt3 = Mathf.Sqrt(3.0f);

        private GameObject cellObject;
        private ResourceLoader loader;

        private short x;
        private short y;
        private short z;
        private int key;

        internal void Init(ResourceLoader loader, short x, short y)
        {
            cellObject = gameObject;
            this.loader = loader;

            this.x = x;
            this.y = y;
            this.z = (short)(-x - y);
            this.key = MakeKey(x, y);

            cellObject.transform.position = new Vector3(CenterX, CenterY);
        }

        protected override void OnDispose()
        {
            loader = null;
            DestroyObject(cellObject);

            base.OnDispose();
        }

        public short X
        {
            get { return x; }
        }

        public short Y
        {
            get { return y; }
        }

        public short Z
        {
            get { return z; }
        }

        public int Key
        {
            get { return key; }
        }

        public float CenterX
        {
            get { return Radius * 1.5f * x; }
        }

        public float CenterY
        {
            get { return Radius * Sqrt3 * (y + 0.5f * x); }
        }

        public void ColorTransform(float r = 1, float g = 1, float b = 1, float a = 1)
        {
            (renderer as SpriteRenderer).color = new Color(r, g, b, a);
        }

        public void ToWhite()
        {
            ColorTransform();
        }

        public void ToRed()
        {
            ColorTransform(1, 0, 0);
        }

        public void ToGreen()
        {
            ColorTransform(0, 1, 0);
        }

        public void ToBlue()
        {
            ColorTransform(0, 0, 1);
        }

        public static int MakeKey(short x, short y)
        {
            return (y << 16) | (ushort)x;
        }
    }
}
