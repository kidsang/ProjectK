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
        public int StartX;
        public int StartY;
        public int EndX;
        public int EndY;
        public float ColorR;
        public float ColorG;
        public float ColorB;
    }

    public class MapCellSetting
    {
        public int X;
        public int Y;
        public int Flags;
    }
}
