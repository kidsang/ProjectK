﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ProjectK.Settings
{
    public class MapSetting
    {
        public string Name;
        public int CellCountX;
        public int CellCountY;

        public MapPathSetting[] Paths = new MapPathSetting[0];
        public MapCellSetting[] Cells = new MapCellSetting[0];
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
    }
}
