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
        public ResourceLoader Loader { get; private set; }

        public string Name { get; protected set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public int CellCountX { get; protected set; }
        public int CellCountY { get; protected set; }

        protected Dictionary<int, MapCell> Cells { get; set; }
        protected List<Vector2> StartLocations { get; set; }
        protected List<Vector2> EndLocations { get; set; }
        protected List<List<Vector2>> Paths { get; set; }
        protected List<GameObject> PathObjectRoots { get; set; }

        internal void Init(ResourceLoader loader)
        {
            MapRoot = gameObject;
            Loader = loader;
            Cells = new Dictionary<int, MapCell>();
            StartLocations = new List<Vector2>();
            EndLocations = new List<Vector2>();
            Paths = new List<List<Vector2>>();
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
            Paths = null;
            MapRoot = null;
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
            int key = MapCell.MakeKey(x, y);
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

        public int GetDistance(int x1, int y1, int x2, int y2)
        {
            int z1 = -x1 - y1;
            int z2 = -x2 - y2;
            return (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) + Mathf.Abs(z1 - z2)) / 2;
        }

        public int GetDistance(Vector2 location1, Vector2 location2)
        {
            return GetDistance((int)location1.x, (int)location1.y, (int)location2.x, (int)location2.y);
        }

        public int GetDistance(MapCell cell1, MapCell cell2)
        {
            return GetDistance(cell1.X, cell1.Y, cell2.X, cell2.Y);
        }

        public void AddStartEnd(Vector2 startLocation, Vector2 endLocation)
        {
            StartLocations.Add(startLocation);
            EndLocations.Add(endLocation);
            Paths.Add(new List<Vector2>());
            PathObjectRoots.Add(null);
        }

        public void RemoveStartEnd(int index)
        {
            StartLocations.RemoveAt(index);
            EndLocations.RemoveAt(index);
            Paths.RemoveAt(index);
            PathObjectRoots.RemoveAt(index);
        }

        public void RemoveStartEndByStart(Vector2 startLocation)
        {
            int index = StartLocations.IndexOf(startLocation);
            if (index >= 0)
                RemoveStartEnd(index);
        }

        public void RemoveStartEndByEnd(Vector2 endLocation)
        {
            int index = EndLocations.IndexOf(endLocation);
            if (index >= 0)
                RemoveStartEnd(index);
        }

        public bool CalculatePath(Vector2 startLocation, Vector2 endLocation, List<Vector2> path)
        {
            Dictionary<MapCell, MapCell> cameFrom = new Dictionary<MapCell,MapCell>();
            Dictionary<MapCell, int> cost = new Dictionary<MapCell, int>();
            SortedList<int, MapCell> frontier = new SortedList<int,MapCell>();
            path.Clear();

            MapCell start = GetCell(startLocation);
            MapCell end = GetCell(endLocation);
            MapCell current = null;
            if (start == null || start.IsObstacle || end == null || end.IsObstacle || start == end)
                return false;

            frontier.Add(0, start);
            cameFrom[start] = start;
            cost[start] = 0;

            while (frontier.Count > 0)
            {
                current = frontier.Values[0];
                frontier.RemoveAt(0);

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
                        frontier.Add(priority, neighbour);
                        cameFrom[neighbour] = current;
                    }
                }
            }

            // build path
            while (current != null)
            {
                path.Add(current.Location);
                cameFrom.TryGetValue(current, out current);
            }
            path.Reverse();

            // merge path
            for (int i = path.Count - 1; i >= 2; --i)
            {
                Vector2 p1 = path[i];
                Vector2 p2 = path[i - 1];
                Vector2 p3 = path[i - 2];
                if ((p3.x - p1.x) * (p2.y - p1.y) == (p3.y - p1.y) * (p2.x - p1.x))
                    path.RemoveAt(i);
            }

            return true;
        }

        public void CalculatePaths()
        {
            for (int i = Paths.Count - 1; i >= 0; --i)
                CalculatePath(StartLocations[i], EndLocations[i], Paths[i]);
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
                List<Vector2> path = Paths[index];
                int count = path.Count;
                if (count > 0)
                {
                    pathObjectRoot = new GameObject();
                    pathObjectRoot.transform.parent = MapRoot.transform;
                    PathObjectRoots[index] = pathObjectRoot;

                    GameObject obj = Loader.LoadPrefab("Map/PathEnd").Instantiate();
                    obj.transform.parent = pathObjectRoot.transform;
                    obj.transform.position = path[count - 1];

                    obj = Loader.LoadPrefab("Map/PathStart").Instantiate();
                    obj.transform.parent = pathObjectRoot.transform;
                    obj.transform.position = path[0];

                    for (int i = 1; i < count - 1; ++i)
                    {
                        obj = Loader.LoadPrefab("Map/PathArrow").Instantiate();
                        obj.transform.parent = pathObjectRoot.transform;
                        obj.transform.position = path[i];
                    }
                }
            }
        }

        public void ToggleShowPaths(bool show, bool update = true)
        {
            for (int i = Paths.Count - 1; i >= 0; --i)
                ToggleShowPath(i, show, update);
        }

        // ----------------
        // todo:

        private MapCell lastCell;

        public void Update()
        {
            if (lastCell != null)
            {
                lastCell.ToWhite();
                lastCell.ShowNeighbours(false);
            }

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastCell = GetCellByWorldXY(worldPoint);
            if (lastCell != null)
            {
                lastCell.ToBlue();
                lastCell.ShowNeighbours(true);
            }
        }
    }
}
