using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class EditorMap : Map
    {
        public void New(MapSetting setting)
        {
            Init(new ResourceLoader());
            Load(setting);
            ResizeMap(setting.CellCountX, setting.CellCountY);
        }

        protected override void OnDispose()
        {
            if (Loader != null)
                Loader.Dispose();

            base.OnDispose();
        }

        public void AddCell(short x, short y)
        {
            // todo:
            GameObject cellObject = Loader.LoadPrefab("Map/MapCell").Instantiate();
            cellObject.transform.parent = MapRoot.transform;
            MapCell cell = cellObject.AddComponent<MapCell>();
            cell.Init(this, x, y);
            Cells.Add(cell.Key, cell);
        }

        public void ResizeMap(short countX, short countY)
        {
            CellCountX = countX;
            CellCountY = countY;
            UpdateMapSize();

            Dictionary<int, MapCell> oldCells = Cells;
            Cells = new Dictionary<int, MapCell>();

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
                        Cells.Add(key, cell);
                    }
                    else
                    {
                        AddCell(x, y);
                    }
                }
            }

            foreach (MapCell cell in oldCells.Values)
                cell.Dispose();

            BuildNeighbours();
        }

        public void ResizeMap(int countX, int countY)
        {
            ResizeMap((short)countX, (short)countY);
        }

        public void Save(string url)
        {

        }
    }
}
