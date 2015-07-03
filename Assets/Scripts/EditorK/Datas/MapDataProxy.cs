using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Settings;

namespace Assets.Scripts.EditorK.Datas
{
    public class MapDataProxy
    {
        private static MapDataProxy instance = new MapDataProxy();
        public static MapDataProxy Instance { get { return instance; } }

        private DataRepository<MapSetting> repo = new DataRepository<MapSetting>();
        private ResourceLoader loader;
        public string DataPath { get; private set; }

        public MapSetting Data
        {
            get { return repo.Data; }
        }

        //public void New(int cellCountX, int cellCountY)
        //{
        //    MapSetting data = new MapSetting();
        //    data.CellCountX = cellCountX;
        //    data.CellCountY = cellCountY;
        //    Load(data);
        //}

        public void Load(string path)
        {
            if (loader != null)
                loader.Dispose();
            loader = new ResourceLoader();

            MapSetting data = loader.LoadJsonFile<MapSetting>(path).Data;
            Load(data, path);
        }

        public void Load(MapSetting data, string path = null)
        {
            DataPath = null;
            repo.Clear();
            repo.Operate(data, EditorEvent.MAP_LOAD, null);
        }
    }
}
