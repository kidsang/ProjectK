﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;
using Vectrosity;

namespace ProjectK
{

    /**
     * 左上角为地图的起始点，其中x = 0，y = 0。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class Map : DisposableBehaviour
    {
        protected Transform MapRoot { get; private set; }
        protected Transform CellRoot { get; private set; }
        protected Transform PathRoot { get; private set; }
        public ResourceLoader Loader { get; private set; }

        public string Name { get; protected set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public int CellCountX { get; protected set; }
        public int CellCountY { get; protected set; }

        public Dictionary<int, MapCell> Cells { get; protected set; }
        public List<Vector2> StartLocations { get; protected set; }
        public List<Vector2> EndLocations { get; protected set; }
        public List<Color> PathColors { get; protected set; }
        public List<List<Vector2>> Paths { get; protected set; }
        public List<GameObject> PathObjectRoots { get; protected set; }
        protected PriorityQueue<MapCell> frontier = new PriorityQueue<MapCell>(); // 用于寻路

        internal void Init(ResourceLoader loader)
        {
            MapRoot = gameObject.transform;
            CellRoot = new GameObject("CellRoot").transform;
            CellRoot.SetParent(MapRoot, false);
            PathRoot = new GameObject("PathRoot").transform;
            PathRoot.SetParent(MapRoot, false);

            Loader = loader;
            Cells = new Dictionary<int, MapCell>();
            StartLocations = new List<Vector2>();
            EndLocations = new List<Vector2>();
            PathColors = new List<Color>();
            Paths = new List<List<Vector2>>();
            PathObjectRoots = new List<GameObject>();
        }

        protected override void OnDispose()
        {
            if (Cells != null)
            {
                foreach (MapCell cell in Cells.Values)
                    cell.Dispose();
                Cells = null;
            }

            StartLocations = null;
            EndLocations = null;
            PathColors = null;
            Paths = null;
            MapRoot = null;
            CellRoot = null;
            PathRoot = null;
            Loader = null;

            base.OnDispose();
        }

        public void Load(string url)
        {
            var res = Loader.LoadJsonFile<MapSetting>(url);
            Load(res.Data);
        }

        public void Load(MapSetting setting)
        {
            Name = setting.Name;
            CellCountX = setting.CellCountX;
            CellCountY = setting.CellCountY;
            UpdateMapSize();

            // TODO
            // Load map cells
            Cells = new Dictionary<int, MapCell>();

            BuildNeighbours();
        }

        protected void UpdateMapSize()
        {
            Width = Mathf.Max(0, CellCountX - 1) * MapCell.HalfWidth * 1.5f;
            Height = Mathf.Max(0, CellCountY - 1) * MapCell.Height;
        }

        protected void BuildNeighbours()
        {
            foreach (MapCell cell in Cells.Values)
                cell.BuildNeighbours();
        }

        public MapCell GetCell(short x, short y)
        {
            int key = MapUtils.MakeKey(x, y);
            MapCell cell = null;
            Cells.TryGetValue(key, out cell);
            return cell;
        }

        public MapCell GetCell(int x, int y)
        {
            return GetCell((short)x, (short)y);
        }

        public MapCell GetCell(Vector2 location)
        {
            return GetCell((short)location.x, (short)location.y);
        }

        public MapCell GetCellByWorldXY(float worldX, float worldY)
        {
            Vector2 location = MapUtils.PositionToLocation(worldX, worldY);
            return GetCell(location);
        }

        public MapCell GetCellByWorldXY(Vector3 worldPoint)
        {
            return GetCellByWorldXY(worldPoint.x, worldPoint.y);
        }

        public int GetDistance(int x1, int y1, int x2, int y2)
        {
            return MapUtils.Distance(x1, y1, x2, y2);
        }

        public int GetDistance(Vector2 location1, Vector2 location2)
        {
            return GetDistance((int)location1.x, (int)location1.y, (int)location2.x, (int)location2.y);
        }

        public int GetDistance(MapCell cell1, MapCell cell2)
        {
            return GetDistance(cell1.X, cell1.Y, cell2.X, cell2.Y);
        }

        public void AddPath(Vector2 startLocation, Vector2 endLocation, Color color)
        {
            StartLocations.Add(startLocation);
            EndLocations.Add(endLocation);
            PathColors.Add(color);
            Paths.Add(new List<Vector2>());
            PathObjectRoots.Add(null);
        }

        public void RemovePath(int index)
        {
            StartLocations.RemoveAt(index);
            EndLocations.RemoveAt(index);
            PathColors.RemoveAt(index);
            Paths.RemoveAt(index);

            if (PathObjectRoots[index] != null)
                Destroy(PathObjectRoots[index]);
            PathObjectRoots.RemoveAt(index);
        }

        public void UpdatePath(int index, Vector2 startLocation, Vector2 endLocation, Color color)
        {
            StartLocations[index] = startLocation;
            EndLocations[index] = endLocation;
            PathColors[index] = color;
            Paths[index].Clear();

            if (PathObjectRoots[index] != null)
                Destroy(PathObjectRoots[index]);
            PathObjectRoots[index] = null;
        }

        public bool CalculatePath(Vector2 startLocation, Vector2 endLocation, List<Vector2> path)
        {
            Dictionary<MapCell, MapCell> cameFrom = new Dictionary<MapCell, MapCell>();
            Dictionary<MapCell, int> cost = new Dictionary<MapCell, int>();
            path.Clear();
            frontier.Clear();

            MapCell start = GetCell(startLocation);
            MapCell end = GetCell(endLocation);
            MapCell current = null;
            if (start == null || start.IsObstacle || end == null || end.IsObstacle || start == end)
                return false;

            frontier.Enqueue(start, 0);
            cameFrom[start] = start;
            cost[start] = 0;

            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (current == end)
                    break;

                foreach (MapCell neighbour in current.Neighbours)
                {
                    if (neighbour == null || neighbour.IsObstacle)
                        continue;

                    int newCost = cost[current] + 1;
                    int oldCost;
                    if (!cost.TryGetValue(neighbour, out oldCost) || newCost < oldCost)
                    {
                        cost[neighbour] = newCost;
                        int priority = newCost + GetDistance(neighbour, end);
                        if (frontier.Contains(neighbour))
                            frontier.UpdatePriority(neighbour, priority);
                        else
                            frontier.Enqueue(neighbour, priority);
                        cameFrom[neighbour] = current;
                    }
                }
            }

            // build path
            while (current != start)
            {
                path.Add(current.Location);
                cameFrom.TryGetValue(current, out current);
            }
            path.Add(start.Location);
            path.Reverse();

            // merge path
            for (int i = path.Count - 2; i >= 1; --i)
            {
                Vector2 p1 = path[i + 1];
                Vector2 p2 = path[i];
                Vector2 p3 = path[i - 1];
                if ((p3.x - p1.x) * (p2.y - p1.y) == (p3.y - p1.y) * (p2.x - p1.x))
                    path.RemoveAt(i);
            }

            return true;
        }

        public void CalculatePath(int index)
        {
            CalculatePath(StartLocations[index], EndLocations[index], Paths[index]);
        }

        public void CalculatePaths()
        {
            for (int i = Paths.Count - 1; i >= 0; --i)
                CalculatePath(i);
        }

        public void ToggleShowPath(int index, bool show, bool update = true)
        {
            GameObject pathObjectRoot = PathObjectRoots[index];
            if (pathObjectRoot && (!show || update))
            {
                Object.Destroy(pathObjectRoot);
                PathObjectRoots[index] = null;
                pathObjectRoot = null;
            }

            if (show && pathObjectRoot == null)
            {
                Color color = PathColors[index];

                pathObjectRoot = new GameObject("PathObjectRoot");
                pathObjectRoot.transform.SetParent(PathRoot.transform, false);
                PathObjectRoots[index] = pathObjectRoot;

                GameObject obj = Loader.LoadPrefab("Map/StartMark").Instantiate();
                obj.transform.SetParent(pathObjectRoot.transform, false);
                obj.transform.position = GetCell(StartLocations[index]).Position;
                obj.GetComponent<SpriteRenderer>().color = color;

                obj = Loader.LoadPrefab("Map/EndMark").Instantiate();
                obj.transform.SetParent(pathObjectRoot.transform, false);
                obj.transform.position = GetCell(EndLocations[index]).Position;
                obj.GetComponent<SpriteRenderer>().color = color;

                List<Vector2> path = Paths[index];
                int count = path.Count;
                if (count > 0)
                {
                    Vector3[] points = new Vector3[count];
                    for (int i = 0; i < count; ++i)
                        points[i] = GetCell(path[i]).Position;
                    VectorLine line = new VectorLine("PathLine", points, null, 2, LineType.Continuous);
                    line.color = color;

                    obj = new GameObject("PathLine");
                    obj.transform.SetParent(pathObjectRoot.transform, false);
                    VectorManager.useDraw3D = true;
                    VectorManager.ObjectSetup(obj, line, Visibility.Dynamic, Brightness.None);
                }
            }
        }

        public void ToggleShowPaths(bool show, bool update = true)
        {
            for (int i = Paths.Count - 1; i >= 0; --i)
                ToggleShowPath(i, show, update);
        }
    }
}
