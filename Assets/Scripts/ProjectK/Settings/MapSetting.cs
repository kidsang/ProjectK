using System;
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

        public int[] Numbers;
        public MapCellSetting[] Cells;
    }

    public class MapCellSetting
    {
        public int X;
        public int Y;
    }
}
