﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    /**
     * MapCell使用Axial坐标系，其中x + y + z = 0，因此只需要x和y的就可以确定一个MapCell。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class MapCell : DisposableBehaviour
    {
        public static readonly float Radius = 1.0f;
        public static readonly float Sqrt3 = Mathf.Sqrt(3.0f);

        protected GameObject CellObject { get; private set; }
        public Map Map { get; private set; }
        public ResourceLoader Loader { get; private set; }

        private short x;
        private short y;
        private short z;
        private int key;

        public bool IsObstacle { get; set; }
        public static readonly int NumNeighbours = 6;
        public MapCell[] Neighbours { get; private set; }

        internal void Init(Map map, short x, short y)
        {
            CellObject = gameObject;
            Map = map;
            Loader = map.Loader;

            this.x = x;
            this.y = y;
            this.z = (short)(-x - y);
            this.key = MakeKey(x, y);

            CellObject.transform.position = new Vector3(CenterX, CenterY);

            Neighbours = new MapCell[NumNeighbours];
        }

        protected override void OnDispose()
        {
            DestroyObject(CellObject);
            CellObject = null;

            Map = null;
            Loader = null;
            Neighbours = null;

            base.OnDispose();
        }

        internal void BuildNeighbours()
        {
            Neighbours[0] = Map.GetCell(x + 1, y - 1);
            Neighbours[1] = Map.GetCell(x + 1, y);
            Neighbours[2] = Map.GetCell(x, y + 1);
            Neighbours[3] = Map.GetCell(x - 1, y + 1);
            Neighbours[4] = Map.GetCell(x - 1, y);
            Neighbours[5] = Map.GetCell(x, y - 1);
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

        public Vector2 Location
        {
            get { return new Vector2(x, y); }
        }

        public static float HalfWidth
        {
            get { return Radius; }
        }

        public static float HalfHeight
        {
            get { return Radius * Sqrt3 * 0.5f;}
        }

        public static float Width
        {
            get { return Radius * 2; }
        }

        public static float Height
        {
            get { return Radius * Sqrt3; }
        }

        public float CenterX
        {
            get { return Radius * 1.5f * x; }
        }

        public float CenterY
        {
            get { return Radius * Sqrt3 * (y + 0.5f * x); }
        }

        public Vector2 Position
        {
            get { return new Vector2(CenterX, CenterY); }
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

        public void ShowNeighbours(bool show)
        {
            foreach (MapCell cell in Neighbours)
            {
                if (cell != null)
                {
                    if (show)
                        cell.ColorTransform(0.5f, 0.5f, 0.5f);
                    else
                        cell.ColorTransform();
                }
            }
        }

        public static int MakeKey(short x, short y)
        {
            return (y << 16) | (ushort)x;
        }
    }
}
