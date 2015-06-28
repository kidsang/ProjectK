using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Maps;
using UnityEngine;

namespace Assets.Scripts.EditorK.Maps
{
    public class EditorMap : Map
    {
        private ResourceLoader loader;

        void Start()
        {
            loader = new ResourceLoader();

            Init(new ResourceLoader());
            ResizeMap(10, 10);
        }

        public override void OnDestroy()
        {
            if (loader != null)
            {
                loader.Dispose();
                loader = null;
            }

            base.OnDestroy();
        }

        public void AddCell(short x, short y)
        {
            // todo:
            GameObject cellObject = Loader.LoadPrefab("Map/MapCell").Instantiate();
            cellObject.transform.parent = MapRoot.transform;
            MapCell cell = cellObject.AddComponent<MapCell>();
            cell.Init(Loader, x, y);
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
        }

        public void Save(string url)
        {

        }
    }
}
