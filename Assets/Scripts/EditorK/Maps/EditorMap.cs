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
        private Transform terrainRoot;
        private Dictionary<MapCellFlag, Transform> terrainRoots = new Dictionary<MapCellFlag, Transform>();

        public void New(MapSetting setting)
        {
            Init(new ResourceLoader());
            terrainRoot = new GameObject("TerrainRoot").transform;
            terrainRoot.SetParent(MapRoot, false);

            //Load(setting);
            ResizeMap(setting.CellCountX, setting.CellCountY);

            EventManager.Instance.Register(this, EditorEvent.MAP_LOAD, OnLoadMap);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATHS, OnUpdatePaths);
            EventManager.Instance.Register(this, EditorEvent.MAP_UPDATE_PATH, OnUpdatePath);
            EventManager.Instance.Register(this, EditorEvent.MAP_TERRAIN_UPDATE, OnUpdateTerrain);
        }

        protected override void OnDispose()
        {
            if (Loader != null)
                Loader.Dispose();

            base.OnDispose();
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

                    int key = MapUtils.MakeKey(x, y);
                    if (oldCells.ContainsKey(key))
                    {
                        MapCell cell = oldCells[key];
                        oldCells.Remove(key);
                        Cells.Add(key, cell);
                    }
                    else
                    {
                        GameObject cellObject = Loader.LoadPrefab("Map/MapCell").Instantiate();
                        cellObject.transform.SetParent(CellRoot, false);
                        MapCell cell = cellObject.AddComponent<MapCell>();
                        cell.Init(this, x, y);
                        Cells.Add(cell.Key, cell);
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

        private void OnUpdatePaths(object[] args)
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

        private void OnUpdatePath(object[] args)
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

        private List<MapCell> GetCellsByFlag(MapCellFlag flag)
        {
            List<MapCell> ret = new List<MapCell>();
            foreach (MapCell cell in Cells.Values)
            {
                if (cell.HasFlag(flag))
                    ret.Add(cell);
            }
            return ret;
        }

        public void ToggleShowTerrain(MapCellFlag flag, bool show)
        {
            Transform root;
            if (show)
            {
                if (!terrainRoots.TryGetValue(flag, out root))
                {
                    root = new GameObject("Terrain_" + Enum.GetName(typeof(MapCellFlag), flag)).transform;
                    root.SetParent(terrainRoot, false);
                    terrainRoots.Add(flag, root);
                }

                int rootIndex = root.GetSiblingIndex();
                root.localPosition = new Vector3(0, 0, rootIndex * -0.1f);
                float scale = 1 - rootIndex * 0.2f;

                Color color = TerrainFlagInfo.GetColorByFlag(flag);
                List<MapCell> cells = GetCellsByFlag(flag);
                int cellCount = cells.Count;
                int childCount = root.childCount;
                for (int i = 0; i < cellCount; ++i)
                {
                    MapCell cell = cells[i];
                    Transform terrainMark;
                    if (i >= childCount)
                    {
                        GameObject terrainMarkObj = Loader.LoadPrefab("Map/TerrainMark").Instantiate();
                        terrainMarkObj.GetComponent<SpriteRenderer>().color = color;
                        terrainMark = terrainMarkObj.transform;
                        terrainMark.SetParent(root, false);
                    }
                    else
                    {
                        terrainMark = root.GetChild(i);
                    }
                    terrainMark.position = cell.Position;
                    terrainMark.Translate(0, 0, -terrainMark.localPosition.z);
                    terrainMark.localScale = new Vector3(scale, scale);
                }

                for (int i = cellCount; i < childCount; ++i)
                    Destroy(root.GetChild(i).gameObject);
            }
            else
            {
                if (terrainRoots.TryGetValue(flag, out root))
                {
                    Destroy(root.gameObject);
                    terrainRoots.Remove(flag);
                }
            }
        }

        private void OnUpdateTerrain(object[] args)
        {
            OnUpdateTerrain(args, true);
        }

        private void OnUpdateTerrain(object[] args, bool showTerrain)
        {
            MapSetting setting = MapDataProxy.Instance.Data;
            Dictionary<int, MapCellSetting> cellSettings = MapUtils.ArrayToDict(setting.Cells);
            for (short j = 0; j < CellCountY; ++j)
            {
                for (short i = 0; i < CellCountX; ++i)
                {
                    short x = i;
                    short y = (short)(j - i / 2);
                    int key = MapUtils.MakeKey(x, y);

                    MapCell cell = GetCell(x, y);
                    cell.Flags = 0;

                    MapCellSetting cellSetting;
                    if (cellSettings.TryGetValue(key, out cellSetting))
                    {
                        cell.Flags = cellSetting.Flags;
                    }
                }
            }

            if (showTerrain)
            {
                InfoMap infos = EditorUtils.GetEventInfos(args);
                MapCellFlag flag = (MapCellFlag)infos["flag"];
                ToggleShowTerrain(flag, true);

                CalculatePaths();
                ToggleShowPaths(true, true);
            }
        }

        private void OnLoadMap(object[] args)
        {
            OnUpdateTerrain(null, false);
            OnUpdatePaths(null);
        }
    }
}
