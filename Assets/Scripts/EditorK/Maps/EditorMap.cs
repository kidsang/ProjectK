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

            EventManager.Instance.Register(this, EditorEvent.MAP_LOAD, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATHS, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATH, OnUpdatePath);
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
            cellObject.transform.SetParent(CellRoot, false);
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

        public void OnUpdatePaths(object[] args)
        {
            MapSetting data = MapDataProxy.Instance.Data;
            int numPathDatas = data.Paths.Length;
            int numPathObjs = PathRoot.childCount;

            for (int i = 0; i < numPathDatas; ++i)
            {
                MapPathSetting pathData = data.Paths[i];
                if (i < numPathObjs)
                {
                    UpdatePath(i, new Vector2(pathData.StartX, pathData.StartY),
                        new Vector2(pathData.EndX, pathData.EndY),
                        new Color(pathData.ColorR, pathData.ColorG, pathData.ColorB));
                }
                else
                {
                    AddPath(new Vector2(pathData.StartX, pathData.StartY),
                        new Vector2(pathData.EndX, pathData.EndY),
                        new Color(pathData.ColorR, pathData.ColorG, pathData.ColorB));
                }
            }

            for (int i = numPathDatas; i < numPathObjs; ++i)
            {
                RemovePath(i);
            }

            CalculatePaths();
            ToggleShowPaths(true, true);
        }

        public void OnUpdatePath(object[] args)
        {
            InfoMap infos = EditorUtils.GetEventInfos(args);
            int index = (int)infos["index"];
            MapSetting data = MapDataProxy.Instance.Data;
            MapPathSetting pathData = data.Paths[index];

            UpdatePath(index, new Vector2(pathData.StartX, pathData.StartY),
                new Vector2(pathData.EndX, pathData.EndY),
                new Color(pathData.ColorR, pathData.ColorG, pathData.ColorB));

            CalculatePath(index);
            ToggleShowPath(index, true, true);
        }
    }
}
