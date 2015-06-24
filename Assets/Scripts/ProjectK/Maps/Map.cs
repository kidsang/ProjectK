using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK.Maps
{

    /**
     * 左上角为地图的起始点，其中x = 0，y = 0。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class Map : DisposableBehaviour
    {
        private GameObject mapRoot;
        private ResourceLoader loader;

        private Dictionary<int, MapCell> cells = new Dictionary<int, MapCell>();

        internal void Init(ResourceLoader loader)
        {
            mapRoot = gameObject;
            this.loader = loader;
        }

        protected override void OnDispose()
        {
            loader = null;
            DestroyObject(mapRoot);

            base.OnDispose();
        }

        public MapCell GetCell(short x, short y)
        {
            int key = MapCell.MakeKey(x, y);
            MapCell cell = null;
            cells.TryGetValue(key, out cell);
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
                dot = loader.LoadPrefab("Map/Dot").Instantiate();

            if (lastCell != null)
                lastCell.ToWhite();

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dot.transform.position = new Vector3(worldPoint.x, worldPoint.y);
            lastCell = GetCellByWorldXY(worldPoint);
            if (lastCell != null)
                lastCell.ToBlue();


        }

        // ----------------
        // for editor

        public void AddCell(short x, short y)
        {
            // todo:
            GameObject cellObject = loader.LoadPrefab("Map/MapCell").Instantiate();
            cellObject.transform.parent = mapRoot.transform;
            MapCell cell = cellObject.AddComponent<MapCell>();
            cell.Init(loader, x, y);
            cells.Add(cell.Key, cell);
        }

        public void ResizeMap(short countX, short countY)
        {
            Dictionary<int, MapCell> oldCells = cells;
            cells = new Dictionary<int, MapCell>();

            for (short j = 0; j < countY; ++j)
            {
                for (short i = 0; i < countX; ++i)
                {
                    short x = i;
                    short y = (short)(j - i / 2);

                    int key = MapCell.MakeKey(x, y);
                    if (oldCells.ContainsKey(key))
                    {
                        MapCell cell = oldCells[key];
                        oldCells.Remove(key);
                        cells.Add(key, cell);
                    }
                    else
                    {
                        AddCell(x, y);
                    }
                }
            }
        }
    }
}
