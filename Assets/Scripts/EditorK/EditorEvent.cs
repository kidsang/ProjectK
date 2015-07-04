using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.EditorK
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
        public static string SCENE_MOUSE_CLICK = DefineEvent("SCENE_MOUSE_CLICK");

        // no args
        public static string SCENE_MOUSE_RIGHT_CLICK = DefineEvent("SCENE_MOUSE_RIGHT_CLICK");

        // no args
        public static string SCENE_MOUSE_IN = DefineEvent("SCENE_MOUSE_IN");

        // no args
        public static string SCENE_MOUSE_OUT = DefineEvent("SCENE_MOUSE_OUT");

        // --------------------------
        // 编辑器数据事件

        // no args
        public static string MAP_LOAD = DefineEvent("MAP_LOAD");

        // no args
        public static string MAP_ADD_PATH = DefineEvent("MAP_ADD_PATH");

        // int index
        public static string MAP_REMOVE_PATH = DefineEvent("MAP_REMOVE_PATH");

        // int index1
        // int index2
        public static string MAP_SWAP_PATH = DefineEvent("MAP_SWAP_PATH");

        // int index
        public static string MAP_SET_PATH_START = DefineEvent("MAP_SET_PATH_START");

        // int index
        public static string MAP_SET_PATH_END = DefineEvent("MAP_SET_PATH_END");

        // int index
        public static string MAP_SET_PATH_COLOR = DefineEvent("MAP_SET_PATH_COLOR");
    }
}
