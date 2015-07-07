using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ProjectK.Base
{
    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4,
    }

    class Log
    {
        public static LogLevel logLevel = LogLevel.Debug;
        private static StringBuilder msgBuilder = new StringBuilder();

        public static void Debug(params object[] args)
        {
            if (logLevel < LogLevel.Debug)
                return;
            Output(LogLevel.Debug, args);
        }

        public static void Info(params object[] args)
        {
            if (logLevel < LogLevel.Info)
                return;
            Output(LogLevel.Info, args);
        }

        public static void Warning(params object[] args)
        {
            if (logLevel < LogLevel.Warning)
                return;
            Output(LogLevel.Warning, args);
        }

        public static void Error(params object[] args)
        {
            if (logLevel < LogLevel.Error)
                return;
            Output(LogLevel.Error, args);
        }

        public static void Assert(bool condition, params object[] args)
        {
            if (condition)
                return;

            msgBuilder.Remove(0, msgBuilder.Length);
            msgBuilder.Append("Assert! ");
            foreach (object arg in args)
            {
                msgBuilder.Append(arg);
                msgBuilder.Append(" ");
            }

            String msg = msgBuilder.ToString();
            throw new Exception(msg);
        }

        private static void Output(LogLevel level, params object[] args)
        {
            msgBuilder.Remove(0, msgBuilder.Length);
            msgBuilder.Append("[");
            msgBuilder.Append(DateTime.Now.ToString());
            msgBuilder.Append("]\n");
            foreach (object arg in args)
            {
                msgBuilder.Append(arg);
                msgBuilder.Append(" ");
            }

            String msg = msgBuilder.ToString();
            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(msg);
                    break;

                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(msg);
                    break;

                case LogLevel.Error:
                    UnityEngine.Debug.LogError(msg);
                    break;
            }
        }
    }
}
