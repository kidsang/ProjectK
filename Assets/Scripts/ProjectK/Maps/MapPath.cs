using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace ProjectK
{
    public class MapPath
    {
        private Map map;
        private List<MapWaypoint> waypoints = new List<MapWaypoint>();

        public void Init(Map map)
        {
            this.map = map;
        }

        public void AddWaypoints(List<Vector2> locations)
        {
            foreach (var location in locations)
            {
                MapWaypoint waypoint = new MapWaypoint();
                waypoint.Location = location;
                waypoint.Position = MapUtils.LocationToPosition(location);
                waypoints.Add(waypoint);
            }
        }

        public void ClearWaypoints()
        {
            waypoints.Clear();
        }

        public List<Vector2> FindPathLocation(int toWaypointIndex)
        {
            List<Vector2> locations;
            int nextLocationIndex;
            FindPathLocation(waypoints[0].Location, toWaypointIndex, out locations, out nextLocationIndex);
            return locations;
        }

        public List<Vector3> FindPathPosition(int toWaypointIndex)
        {
            List<Vector3> positions;
            int nextPositionIndex;
            FindPathPosition(waypoints[0].Location, toWaypointIndex, out positions, out nextPositionIndex);
            return positions;
        }

        public void FindPathLocation(Vector2 fromLocation, int toWaypointIndex, out List<Vector2> locations, out int nextLocationIndex)
        {
            int pathIndex = FindPathIndex(fromLocation, toWaypointIndex, out nextLocationIndex);
            MapWaypoint waypoint = waypoints[toWaypointIndex];
            locations = waypoint.LocationPaths[pathIndex];
        }

        public void FindPathPosition(Vector2 fromLocation, int toWaypointIndex, out List<Vector3> positions, out int nextPositionIndex)
        {
            int pathIndex = FindPathIndex(fromLocation, toWaypointIndex, out nextPositionIndex);
            MapWaypoint waypoint = waypoints[toWaypointIndex];
            positions = waypoint.PositionPaths[pathIndex];
        }

        private int FindPathIndex(Vector2 fromLocation, int toWaypointIndex, out int nextLocationIndex)
        {
            Log.Assert(toWaypointIndex >= 0 || toWaypointIndex < waypoints.Count, "寻路路点输入错误！ toWaypointIndex:", toWaypointIndex, "waypoints.Count:", waypoints.Count);

            MapWaypoint waypoint = waypoints[toWaypointIndex];
            int pathsCount = waypoint.LocationPaths.Count;
            for (int i = 0; i < pathsCount; i++)
            {
                List<Vector2> path = waypoint.LocationPaths[i];
                int pathCount = path.Count;
                for (int j = 1; j < pathCount; j++)
                {
                    Vector2 p0 = path[j - 1];
                    Vector2 p1 = path[j];
                    if (MapUtils.InSegment(fromLocation, p0, p1))
                    {
                        nextLocationIndex = j;
                        return i;
                    }
                }
            }

            List<Vector2> locationPath = new List<Vector2>();
            map.CalculatePath(fromLocation, waypoint.Location, locationPath);
            waypoint.LocationPaths.Add(locationPath);

            int count = locationPath.Count;
            List<Vector3> positionPath = new List<Vector3>(count);
            for (int i = 0; i < count; i++)
                positionPath.Add(MapUtils.LocationToPosition(locationPath[i]));
            waypoint.PositionPaths.Add(positionPath);

            nextLocationIndex = 1;
            return waypoint.LocationPaths.Count - 1;
        }

        public void ClearPath(int toWaypointIndex)
        {
            MapWaypoint waypoint = waypoints[toWaypointIndex];
            waypoint.LocationPaths.Clear();
            waypoint.PositionPaths.Clear();
        }

        public void ClearAllPaths()
        {
            for (int i = 1; i < waypoints.Count; i++)
                ClearPath(i);
        }

        public void RecalculatePath(int toWaypointIndex)
        {
            MapWaypoint waypoint = waypoints[toWaypointIndex];
            waypoint.LocationPaths.Clear();
            waypoint.PositionPaths.Clear();
            int nextLocaitonIndex;
            FindPathIndex(waypoints[toWaypointIndex - 1].Location, toWaypointIndex, out nextLocaitonIndex);
        }

        public void RecalculatePaths()
        {
            for (int i = 1; i < waypoints.Count; ++i)
                RecalculatePath(i);
        }

        public int WaypointCount
        {
            get { return waypoints.Count; }
        }

        public Vector2 GetLocation(int index)
        {
            MapWaypoint waypoint = waypoints[index];
            return waypoint.Location;
        }

        public Vector2 GetPosition(int index)
        {
            MapWaypoint waypoint = waypoints[index];
            return waypoint.Position;
        }

        public Vector2 StartLocation
        {
            get { return GetLocation(0); }
        }

        public Vector2 EndLocation
        {
            get { return GetLocation(waypoints.Count - 1); }
        }

        public Vector2 StartPosition
        {
            get { return GetPosition(0); }
        }

        public Vector2 EndPosition
        {
            get { return GetPosition(waypoints.Count - 1); }
        }

        class MapWaypoint
        {
            public Vector2 Location;
            public Vector3 Position;

            public List<List<Vector2>> LocationPaths = new List<List<Vector2>>();
            public List<List<Vector3>> PositionPaths = new List<List<Vector3>>();
        }
    }
}
