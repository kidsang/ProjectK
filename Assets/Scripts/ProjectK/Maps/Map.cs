using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;

namespace Assets.Scripts.ProjectK.Maps
{

    /**
     * 左上角为地图的起始点，其中x = 0，y = 0。
     * 参见 http://www.redblobgames.com/grids/hexagons
     */
    public class Map : DisposableBehaviour
    {
        private GameObject mapRoot;
        private ResourceLoader loader;

        private Dictionary<int, MapCell> cells = new Dictionary<int, MapCell>();

        internal void Init(ResourceLoader loader)
        {
            mapRoot = gameObject;
            this.loader = loader;
        }

        protected override void OnDispose()
        {
            loader = null;
            DestroyObject(mapRoot);

            base.OnDispose();
        }

        public MapCell GetCell(short x, short y)
        {
            int key = MapCell.MakeKey(x, y);
            MapCell cell = null;
            cells.TryGetValue(key, out cell);
            return cell;
        }

        public MapCell GetCellBySceneXY(float sceneX, float sceneY)
        {
            short x = (short)(sceneX * 2 / 3.0f / MapCell.Radius);
            short y = (short)((MapCell.Sqrt3 * sceneY - sceneX) / 3.0f / MapCell.Radius);
            return GetCell(x, y);
        }

        public MapCell GetCellBySceneXY(Vector3 scenePos)
        {
            return GetCellBySceneXY(scenePos.x, scenePos.y);
        }

        // ----------------
        // for editor

        public void AddCell(short x, short y)
        {
            // todo:
            GameObject cellObject = Instantiate(Resources.Load<GameObject>("Map/MapCell")) as GameObject;
            cellObject.transform.parent = mapRoot.transform;
            MapCell cell = cellObject.AddComponent<MapCell>();
            cell.Init(loader, x, y);
            cells.Add(cell.Key, cell);
        }

        public void ResizeMap(short countX, short countY)
        {
            Dictionary<int, MapCell> oldCells = cells;
            cells = new Dictionary<int, MapCell>();

            for (short j = 0; j < countY; ++j)
            {
                for (short i = 0; i < countX; ++i)
                {
                    short x = i;
                    short y = (short)(j - i / 2);

                    int key = MapCell.MakeKey(x, y);
                    if (oldCells.ContainsKey(key))
                    {
                        MapCell cell = oldCells[key];
                        oldCells.Remove(key);
                        cells.Add(key, cell);
                    }
                    else
                    {
                        AddCell(x, y);
                    }
                }
            }
        }
    }
}
