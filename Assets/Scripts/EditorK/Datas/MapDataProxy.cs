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

        public string DataPath { get; private set; }
        public MapSetting Data { get { return repo.Data; } }

        private DataRepository<MapSetting> repo = new DataRepository<MapSetting>();

        public void Load(MapSetting data, string path = null)
        {
            DataPath = path;
            repo.New(data, EditorEvent.MAP_LOAD, null);
        }
    }
}
