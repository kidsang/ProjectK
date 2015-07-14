using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK;
using ProjectK.Base;

namespace EditorK
{
    public class MapDataProxy
    {
        private static MapDataProxy instance = new MapDataProxy();
        public static MapDataProxy Instance { get { return instance; } }

        public string DataPath { get; private set; }
        public MapSetting Data { get { return repo.Data; } }

        private DataRepository<MapSetting> repo = new DataRepository<MapSetting>();

        public bool Recording
        {
            get { return repo.Recording; }
            set { repo.Recording = value; }
        }

        public void Undo()
        {
            repo.Undo();
        }

        public void Redo()
        {
            repo.Redo();
        }

        public void Load(MapSetting data, string path = null)
        {
            DataPath = path;
            repo.New(data, EditorEvent.MAP_LOAD, null);
        }

        private void Modify(string evt, InfoMap infos)
        {
            repo.Modify(evt, infos);
            GameEditor.Instance.FileModified = true;
        }

        public void AddPath(int startX, int startY, int endX, int endY)
        {
            List<MapPathSetting> paths = new List<MapPathSetting>(Data.Paths);
            MapPathSetting path = new MapPathSetting();
            path.StartX = startX;
            path.StartY = startY;
            path.EndX = endX;
            path.EndY = endY;
            path.ColorR = Random.value;
            path.ColorG = Random.value;
            path.ColorB = Random.value;
            paths.Add(path);
            Data.Paths = paths.ToArray();

            Modify(EditorEvent.MAP_UPDATE_PATHS, null);
        }

        public void RemovePath(int index)
        {
            List<MapPathSetting> paths = new List<MapPathSetting>(Data.Paths);
            paths.RemoveAt(index);
            Data.Paths = paths.ToArray();

           Modify(EditorEvent.MAP_UPDATE_PATHS, null);
        }

        public void SwapPath(int index1, int index2)
        {
            List<MapPathSetting> paths = new List<MapPathSetting>(Data.Paths);
            MapPathSetting temp = paths[index1];
            paths[index1] = paths[index2];
            paths[index2] = temp;
            Data.Paths = paths.ToArray();

            Modify(EditorEvent.MAP_UPDATE_PATHS, null);
        }

        public void SetPathStart(int index, int startX, int startY)
        {
            MapPathSetting path = Data.Paths[index];
            path.StartX = startX;
            path.StartY = startY;

            InfoMap infos = new InfoMap();
            infos["index"] = index;
            Modify(EditorEvent.MAP_UPDATE_PATH, infos);
        }

        public void SetPathEnd(int index, int endX, int endY)
        {
            MapPathSetting path = Data.Paths[index];
            path.EndX = endX;
            path.EndY = endY;

            InfoMap infos = new InfoMap();
            infos["index"] = index;
            Modify(EditorEvent.MAP_UPDATE_PATH, infos);
        }

        public void SetPathColor(int index, float colorR, float colorG, float colorB)
        {
            MapPathSetting path = Data.Paths[index];
            path.ColorR = colorR;
            path.ColorG = colorG;
            path.ColorB = colorB;

            InfoMap infos = new InfoMap();
            infos["index"] = index;
            Modify(EditorEvent.MAP_UPDATE_PATH, infos);
        }

        public void SetTerrainFlag(int x, int y, int radius, MapCellFlag flag, bool apply)
        {
            Dictionary<int, MapCellSetting> cellSettings = MapUtils.ArrayToDict(Data.Cells);
            EditorMap map = GameEditor.Instance.Map;
            Vector2[] locations = MapUtils.Circle(x, y, radius);
            foreach (Vector2 location in locations)
            {
                MapCell cell = map.GetCell(location);
                if (cell == null)
                    continue;

                if (apply)
                {
                    MapCellSetting cellSetting;
                    if (!cellSettings.TryGetValue(cell.Key, out cellSetting))
                    {
                        cellSetting = new MapCellSetting();
                        cellSetting.X = cell.X;
                        cellSetting.Y = cell.Y;
                        cellSettings.Add(cell.Key, cellSetting);
                    }
                    EditorUtils.SetFlag(ref cellSetting.Flags, (int)flag, apply);
                }
                else
                {
                    MapCellSetting cellSetting;
                    if (cellSettings.TryGetValue(cell.Key, out cellSetting))
                    {
                        EditorUtils.SetFlag(ref cellSetting.Flags, (int)flag, apply);
                        if (cellSetting.Flags == 0)
                            cellSettings.Remove(cell.Key);
                    }
                }
            }

            Data.Cells = MapUtils.DictToArray(cellSettings);

            InfoMap infos = new InfoMap();
            infos["flag"] = flag;
            Modify(EditorEvent.MAP_TERRAIN_UPDATE, infos);
        }
    }
}
