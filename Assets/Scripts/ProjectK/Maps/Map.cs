using System.Collections.Generic;
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
        public List<Color> PathColors { get; protected set; }
        public List<GameObject> PathObjectRoots { get; protected set; }
        public List<MapPath> Paths { get; protected set; }
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
            PathColors = new List<Color>();
            PathObjectRoots = new List<GameObject>();
            Paths = new List<MapPath>();
        }

        protected override void OnDispose()
        {
            if (Cells != null)
            {
                foreach (MapCell cell in Cells.Values)
                    cell.Dispose();
                Cells = null;
            }

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

            // Load map cells
            Cells = new Dictionary<int, MapCell>();
            foreach (MapCellSetting cellSetting in setting.Cells)
            {
                GameObject cellObject = Loader.LoadPrefab("Map/MapCell").Instantiate();
                cellObject.transform.SetParent(CellRoot, false);
                MapCell cell = cellObject.AddComponent<MapCell>();
                cell.Init(this, (short)cellSetting.X, (short)cellSetting.Y);
                cell.Load(cellSetting);
                Cells.Add(cell.Key, cell);
            }
            BuildNeighbours();

            // Load paths
            foreach (MapPathSetting pathSetting in setting.Paths)
            {
                List<Vector2> locations = new List<Vector2>(pathSetting.Waypoints.Length);
                foreach (var point in pathSetting.Waypoints)
                    locations.Add(new Vector2(point.X, point.Y));
                AddPath(locations, new Color(pathSetting.ColorR, pathSetting.ColorG, pathSetting.ColorB));
            }
            CalculatePaths();
            ToggleShowPaths(true);
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

        public void AddPath(List<Vector2> locations, Color color)
        {
            MapPath path = new MapPath();
            path.Init(this);
            path.AddWaypoints(locations);
            Paths.Add(path);

            PathColors.Add(color);
            PathObjectRoots.Add(null);
        }

        public void RemovePath(int index)
        {
            PathColors.RemoveAt(index);
            Paths.RemoveAt(index);

            if (PathObjectRoots[index] != null)
                Destroy(PathObjectRoots[index]);
            PathObjectRoots.RemoveAt(index);
        }

        public void UpdatePath(int index, List<Vector2> locations, Color color)
        {
            PathColors[index] = color;
            Paths[index].ClearWaypoints();
            Paths[index].AddWaypoints(locations);

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
                if (MapUtils.InLine(p2, p1, p3))
                    path.RemoveAt(i);
            }

            return true;
        }

        public void CalculatePath(int index)
        {
            MapPath path = Paths[index];
            path.RecalculatePaths();
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
                MapPath path = Paths[index];

                pathObjectRoot = new GameObject("PathObjectRoot");
                pathObjectRoot.transform.SetParent(PathRoot.transform, false);
                PathObjectRoots[index] = pathObjectRoot;

                GameObject obj = Loader.LoadPrefab("Map/StartMark").Instantiate();
                obj.transform.SetParent(pathObjectRoot.transform, false);
                obj.transform.position = path.StartPosition;
                obj.GetComponent<SpriteRenderer>().color = color;

                obj = Loader.LoadPrefab("Map/EndMark").Instantiate();
                obj.transform.SetParent(pathObjectRoot.transform, false);
                obj.transform.position = path.EndPosition;
                obj.GetComponent<SpriteRenderer>().color = color;

                int count = path.WaypointCount;
                for (int i = 1; i < count; ++i)
                {
                    List<Vector3> positions = path.FindPathPosition(i);
                    VectorLine line = new VectorLine("PathLine", positions, null, 2, LineType.Continuous);
                    line.color = color;

                    obj = new GameObject("PathLine");
                    obj.transform.SetParent(pathObjectRoot.transform, false);
                    VectorManager.useDraw3D = true;
                    VectorManager.ObjectSetup(obj, line, Visibility.Dynamic, Brightness.None);

                    if (i < count - 1)
                    {
                        obj = Loader.LoadPrefab("Map/StartMark").Instantiate();
                        obj.transform.SetParent(pathObjectRoot.transform, false);
                        obj.transform.position = path.GetPosition(i);
                        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        obj.GetComponent<SpriteRenderer>().color = color;
                    }
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
