using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Settings;

namespace Assets.Scripts.ProjectK.Maps
{

    /**
     * 左上角为地图的起始点，其中x = 0，y = 0。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class Map : DisposableBehaviour
    {
        protected GameObject MapRoot { get; private set; }
        protected ResourceLoader Loader { get; private set; }

        public string Name { get; protected set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public int CellCountX { get; protected set; }
        public int CellCountY { get; protected set; }

        protected Dictionary<int, MapCell> Cells { get; set; }

        internal void Init(ResourceLoader loader)
        {
            MapRoot = gameObject;
            Loader = loader;
            Cells = new Dictionary<int, MapCell>();
        }

        protected override void OnDispose()
        {
            foreach (MapCell cell in Cells.Values)
                cell.Dispose();
            Cells = null;

            Loader = null;

            DestroyObject(MapRoot);
            MapRoot = null;

            base.OnDispose();
        }

        public void Load(string url)
        {
            var res = Loader.LoadJsonFile<MapSetting>(url);
            MapSetting setting = res.Data;

            Name = setting.Name;
            CellCountX = setting.CellCountX;
            CellCountY = setting.CellCountY;
            UpdateMapSize();
        }

        protected void UpdateMapSize()
        {
            Width = Mathf.Max(0, CellCountX - 1) * MapCell.HalfWidth * 1.5f;
            Height = Mathf.Max(0, CellCountY - 1) * MapCell.Height;
        }

        public MapCell GetCell(short x, short y)
        {
            int key = MapCell.MakeKey(x, y);
            MapCell cell = null;
            Cells.TryGetValue(key, out cell);
            return cell;
        }

        public MapCell GetCellByWorldXY(float worldX, float worldY)
        {
            float fx = worldX * 2 / 3.0f / MapCell.Radius;
            float fy = (MapCell.Sqrt3 * worldY - worldX) / 3.0f / MapCell.Radius;
            float fz = -fx - fy;

            float rx = Mathf.Round(fx);
            float ry = Mathf.Round(fy);
            float rz = Mathf.Round(fz);

            float dx = Mathf.Abs(rx - fx);
            float dy = Mathf.Abs(ry - fy);
            float dz = Mathf.Abs(rz - fz);

            if (dx > dy && dx > dz)
                rx = -ry - rz;
            else if (dy > dz)
                ry = -rx - rz;

            short x = (short)rx;
            short y = (short)ry;
            return GetCell(x, y);
        }

        public MapCell GetCellByWorldXY(Vector3 worldPoint)
        {
            return GetCellByWorldXY(worldPoint.x, worldPoint.y);
        }

        // ----------------
        // todo:

        private MapCell lastCell;
        private GameObject dot;

        public void Update()
        {
            if (dot == null)
                dot = Loader.LoadPrefab("Map/Dot").Instantiate();

            if (lastCell != null)
                lastCell.ToWhite();

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dot.transform.position = new Vector3(worldPoint.x, worldPoint.y);
            lastCell = GetCellByWorldXY(worldPoint);
            if (lastCell != null)
                lastCell.ToBlue();


        }
    }
}
