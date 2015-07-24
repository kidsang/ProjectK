using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    public class MapSetting
    {
        public string Name;
        public int CellCountX;
        public int CellCountY;

        public MapPathSetting[] Paths;
        public MapCellSetting[] Cells;
    }

    public class MapPathSetting
    {
        public float ColorR;
        public float ColorG;
        public float ColorB;
        public MapWaypointSetting[] Waypoints;
    }

    public class MapWaypointSetting
    {
        public int X;
        public int Y;
    }

    public class MapCellSetting
    {
        public int X;
        public int Y;
        public int Flags;
    }
}
