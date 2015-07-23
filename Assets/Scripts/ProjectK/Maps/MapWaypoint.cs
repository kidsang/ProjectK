using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class MapWaypoint
    {
        public Vector2 Location;
        public Vector3 Position;

        public List<List<Vector2>> LocationPaths = new List<List<Vector2>>();
        public List<List<Vector3>> PositionPaths = new List<List<Vector3>>();
    }
}
