using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class MapPath
    {
        public List<MapWaypoint> Waypoints = new List<MapWaypoint>();
        private Map map;

        public void Init(Map map)
        {
            this.map = map;
        }

        public List<Vector2> FindPath(Vector2 fromLocation, int toWaypointIndex)
        {
            if (toWaypointIndex == 0 || toWaypointIndex >= Waypoints.Count)
                return new List<Vector2>();

            MapWaypoint waypoint = Waypoints[toWaypointIndex];
            int pathIndex = FindPathIndex(fromLocation, toWaypointIndex);
            return waypoint.LocationPaths[pathIndex];
        }

        public List<Vector3> FindPath(Vector3 fromLocation, int toWaypointIndex)
        {
            if (toWaypointIndex == 0 || toWaypointIndex >= Waypoints.Count)
                return new List<Vector3>();

            MapWaypoint waypoint = Waypoints[toWaypointIndex];
            int pathIndex = FindPathIndex(fromLocation, toWaypointIndex);
            return waypoint.PositionPaths[pathIndex];
        }

        public int FindPathIndex(Vector2 fromLocation, int toWaypointIndex)
        {
            MapWaypoint waypoint = Waypoints[toWaypointIndex];
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
                        return i;
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

            return waypoint.LocationPaths.Count - 1;
        }

        public void ClearPath(int toWaypointIndex)
        {
            if (toWaypointIndex == 0 || toWaypointIndex >= Waypoints.Count)
                return;

            MapWaypoint waypoint = Waypoints[toWaypointIndex];
            waypoint.LocationPaths.Clear();
            waypoint.PositionPaths.Clear();
        }

        public void ClearAllPaths()
        {
            for (int i = 1; i < Waypoints.Count; i++)
                ClearPath(i);
        }
    }
}
