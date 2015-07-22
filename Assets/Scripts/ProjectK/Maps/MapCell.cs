using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public enum MapCellFlag
    {
        None = 0,
        CanWalk = 1,
        CanBuild = 2,
    }

    /**
     * MapCell使用Axial坐标系，其中x + y + z = 0，因此只需要x和y的就可以确定一个MapCell。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class MapCell : DisposableBehaviour, IPriorityQueueNode
    {
        public static readonly float Radius = MapUtils.Radius;
        public static readonly float Sqrt3 = Mathf.Sqrt(3.0f);

        protected GameObject CellObject { get; private set; }
        public Map Map { get; private set; }
        public ResourceLoader Loader { get; private set; }

        public short X { get; private set; }
        public short Y { get; private set; }
        public short Z { get; private set; }
        public int Key { get; private set; }
        public int Flags { get; set; }

        public static readonly int NumNeighbours = 6;
        public MapCell[] Neighbours { get; private set; }

        // implements IPriorityQueueNode 
        public double Priority { get; set; }
        public long InsertionIndex { get; set; }
        public int QueueIndex { get; set; }

        internal void Init(Map map, short x, short y)
        {
            CellObject = gameObject;
            Map = map;
            Loader = map.Loader;

            this.X = x;
            this.Y = y;
            this.Z = (short)(-x - y);
            this.Key = MapUtils.MakeKey(x, y);

            CellObject.transform.localPosition = new Vector3(CenterX, CenterY);

            Neighbours = new MapCell[NumNeighbours];
        }

        internal void Load(MapCellSetting setting)
        {
            Flags = setting.Flags;
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
            Neighbours[0] = Map.GetCell(X + 1, Y);
            Neighbours[1] = Map.GetCell(X + 1, Y - 1);
            Neighbours[2] = Map.GetCell(X, Y - 1);
            Neighbours[3] = Map.GetCell(X - 1, Y);
            Neighbours[4] = Map.GetCell(X - 1, Y + 1);
            Neighbours[5] = Map.GetCell(X, Y + 1);
        }

        public Vector2 Location
        {
            get { return new Vector2(X, Y); }
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
            get { return Radius * 1.5f * X; }
        }

        public float CenterY
        {
            get { return Radius * Sqrt3 * (Y + 0.5f * X); }
        }

        public Vector3 Position
        {
            get { return new Vector3(CenterX, CenterY); }
        }

        public bool HasFlag(MapCellFlag flag)
        {
            return (Flags & (int)flag) != 0;
        }

        public void SetFlag(MapCellFlag flag, bool value)
        {
            if (value)
                Flags = Flags | (int)flag;
            else
                Flags = Flags & (~(int)flag);
        }

        public bool IsObstacle
        {
            get { return !HasFlag(MapCellFlag.CanWalk); }
        }

        public bool CanWalk
        {
            get { return HasFlag(MapCellFlag.CanWalk); }
            set { SetFlag(MapCellFlag.CanWalk, value); }
        }

        public bool CanBuild
        {
            get { return HasFlag(MapCellFlag.CanBuild); }
            set { SetFlag(MapCellFlag.CanBuild, value); }
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

        public override string ToString()
        {
            return "MapCell(" + X + "," + Y + ")";
        }
    }
}
