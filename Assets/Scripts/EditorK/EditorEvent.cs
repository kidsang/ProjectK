using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorK
{
    public class EditorEvent
    {
        private static string DefineEvent(string name)
        {
            return "Editor_" + name;
        }

        // no args
        public static string SCREEN_RESIZE = DefineEvent("SCREEN_RESIZE");

        // no args
        public static string SCENE_MOUSE_DOWN = DefineEvent("SCENE_MOUSE_DOWN");

        // no args
        public static string SCENE_MOUSE_RIGHT_DOWN = DefineEvent("SCENE_MOUSE_RIGHT_DOWN");

        // no args
        public static string SCENE_MOUSE_UP = DefineEvent("SCENE_MOUSE_UP");

        // no args
        public static string SCENE_MOUSE_CLICK = DefineEvent("SCENE_MOUSE_CLICK");

        // no args
        public static string SCENE_MOUSE_RIGHT_CLICK = DefineEvent("SCENE_MOUSE_RIGHT_CLICK");

        // no args
        public static string SCENE_MOUSE_IN = DefineEvent("SCENE_MOUSE_IN");

        // no args
        public static string SCENE_MOUSE_OUT = DefineEvent("SCENE_MOUSE_OUT");

        // no args
        public static string SCENE_MOUSE_OVER_CELL_CHANGE = DefineEvent("SCENE_MOUSE_OVER_CELL_CHANGE");

        // no args
        public static string CAMERA_ZOOM_CHANGE = DefineEvent("CAMERA_ZOOM_CHANGE");

        // no args
        public static string MOUSE_SET_DATA = DefineEvent("MOUSE_SET_DATA");

        // no args
        public static string MOUSE_CLEAR_DATA = DefineEvent("MOUSE_CLEAR_DATA");

        // --------------------------
        // 编辑器数据事件

        // no args
        public static string MAP_LOAD = DefineEvent("MAP_LOAD");

        // no args
        public static string MAP_UPDATE_PATHS = DefineEvent("MAP_UPDATE_PATHS");

        // int index
        public static string MAP_UPDATE_PATH = DefineEvent("MAP_UPDATE_PATH");

        // MapCellFlag flag
        public static string MAP_TERRAIN_UPDATE = DefineEvent("MAP_TERRAIN_UPDATE");
    }
}
