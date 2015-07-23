using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK
{
    public static class MapUtils
    {
        public static readonly float Radius = 1.0f;
        public static readonly float Sqrt3 = Mathf.Sqrt(3.0f);
        public static readonly Vector2[] Directions = {
            new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
            new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1)
                                                      };
        public static int MakeKey(short x, short y)
        {
            return (y << 16) | (ushort)x;
        }

        public static Vector3 LocationToPosition(int x, int y)
        {
            return new Vector3(Radius * 1.5f * x, Radius * Sqrt3 * (y + 0.5f * x));
        }

        public static Vector3 LocationToPosition(Vector2 location)
        {
            return LocationToPosition((int)location.x, (int)location.y);
        }

        public static Vector2 PositionToLocation(float x, float y)
        {
            float fx = x * 2 / 3.0f / Radius;
            float fy = (Sqrt3 * y - x) / 3.0f / Radius;
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

            return new Vector2((int)rx, (int)ry);
        }

        public static Vector2 PositionToLocation(Vector3 position)
        {
            return PositionToLocation(position.x, position.y);
        }

        public static bool InLine(Vector2 p, Vector2 p1, Vector2 p2)
        {
            return (p2.x - p1.x) * (p.y - p1.y) == (p2.y - p1.y) * (p.x - p1.x);
        }

        public static bool InSegment(Vector2 p, Vector2 p1, Vector2 p2)
        {
            if (!InLine(p, p1, p2))
                return false;

            if (p1.x < p2.x)
            {
                if (p.x < p1.x || p.x > p2.x)
                    return false;
            }
            else
            {
                if (p.x > p1.x || p.x < p2.x)
                    return false;
            }

            if (p1.y < p2.y)
            {
                if (p.y < p1.y || p.y > p2.y)
                    return false;
            }
            else
            {
                if (p.y > p1.y || p.y < p2.y)
                    return false;
            }

            return true;
        }

        public static int Distance(int x1, int y1, int x2, int y2)
        {
            int z1 = -x1 - y1;
            int z2 = -x2 - y2;
            return (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) + Mathf.Abs(z1 - z2)) / 2;
        }

        public static int Distance(Vector2 location1, Vector2 location2)
        {
            return Distance((int)location1.x, (int)location1.y, (int)location2.x, (int)location2.y);
        }

        public static Vector2 Neighbour(int x, int y, int direction)
        {
            direction %= Directions.Length;
            Vector2 dir = Directions[direction];
            return new Vector2(dir.x + x, dir.y + y);
        }

        public static Vector2 Neighbour(Vector2 location, int direction)
        {
            return Neighbour((int)location.x, (int)location.y, direction);
        }

        public static Vector2 Neighbour(int x, int y, int direction, int distance)
        {
            direction %= Directions.Length;
            Vector2 dir = Directions[direction];
            return new Vector2(dir.x * distance + x, dir.y * distance + y);
        }

        public static Vector2 Neighbour(Vector2 location, int direction, int distance)
        {
            return Neighbour((int)location.x, (int)location.y, direction, distance);
        }

        public static Vector2[] Neighbours(int x, int y)
        {
            int count = Directions.Length;
            Vector2[] ret = new Vector2[count];
            Directions.CopyTo(ret, count);
            for (int i = 0; i < count; ++i)
            {
                ret[i].x += x;
                ret[i].y += y;
            }
            return ret;
        }

        private static void Ring(int x, int y, int radius, Vector2[] ret, ref int index)
        {
            if (radius > 0)
            {
                Vector2 current = Neighbour(x, y, 4, radius);
                for (int i = 0; i < 6; ++i)
                {
                    for (int j = 0; j < radius; ++j)
                    {
                        ret[index++] = current;
                        current = Neighbour(current, i);
                    }
                }
            }
            else
            {
                ret[index++] = new Vector2(x, y);
            }
        }

        public static Vector2[] Ring(int x, int y, int radius)
        {
            int count = radius <= 0 ? 1 : radius * 6;
            Vector2[] ret = new Vector2[count];

            int index = 0;
            Ring(x, y, radius, ret, ref index);

            return ret;
        }

        public static Vector2[] Circle(int x, int y, int radius)
        {
            int count = 1;
            for (int i = 1; i <= radius; ++i)
                count += 6 * i;
            Vector2[] ret = new Vector2[count];

            int index = 0;
            for (int i = 0; i <= radius; ++i)
                Ring(x, y, i, ret, ref index);

            return ret;
        }

        public static Dictionary<int, MapCellSetting> ArrayToDict(MapCellSetting[] cellSettings)
        {
            Dictionary<int, MapCellSetting> ret = new Dictionary<int, MapCellSetting>();
            foreach (MapCellSetting cellSetting in cellSettings)
                ret[MapUtils.MakeKey((short)cellSetting.X, (short)cellSetting.Y)] = cellSetting;
            return ret;
        }

        public static MapCellSetting[] DictToArray(Dictionary<int, MapCellSetting> cellSettings)
        {
            return cellSettings.Values.ToArray();
        }
    }
}
