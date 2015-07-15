using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public class TerrainFlagInfo
    {
        public MapCellFlag Flag;
        public string Name;
        public string FlagName;
        public Color Color;

        public TerrainFlagInfo(MapCellFlag flag, string name, Color color)
        {
            Flag = flag;
            Name = name;
            FlagName = Enum.GetName(typeof(MapCellFlag), flag);
            Color = color;
        }

        public static TerrainFlagInfo[] Infos = {
            new TerrainFlagInfo(MapCellFlag.CanWalk, "可通过", new Color(0.5f, 0.5f, 1)),
            new TerrainFlagInfo(MapCellFlag.CanBuild, "可建造", new Color(1, 0.5f, 0.3f)),
                                                };

        public static TerrainFlagInfo GetInfoByFlag(MapCellFlag flag)
        {
            foreach (TerrainFlagInfo info in Infos)
            {
                if (info.Flag == flag)
                    return info;
            }
            return null;
        }

        public static string GetNameByFlag(MapCellFlag flag)
        {
            TerrainFlagInfo info = GetInfoByFlag(flag);
            if (info != null)
                return info.Name;
            return "未知地形";
        }

        public static Color GetColorByFlag(MapCellFlag flag)
        {
            TerrainFlagInfo info = GetInfoByFlag(flag);
            if (info != null)
                return info.Color;
            return Color.black;
        }
    }
}
