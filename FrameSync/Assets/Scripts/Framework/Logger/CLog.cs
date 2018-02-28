using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class CLog
    {
        //public static LogRecorder logRecorder = new LogRecorder();

        public static void Init() { }

        public static void Log(string msg, string color = "#ffffff")
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(color))
            {
                sb.Append(msg.ToString());
            }
            else
            {
                sb.Append("<color=" + color + ">");
                sb.Append(msg.ToString());
                sb.Append("</color>");
            }

            Debug.Log(sb.ToString());
            //logRecorder.Log(msg);
        }

        public static void LogError(object msg)
        {
            UnityEngine.Debug.LogError(msg);
            //logRecorder.LogError(msg);
        }

        public static void LogWarn(object msg)
        {
            UnityEngine.Debug.LogWarning(msg);
            //logRecorder.LogWarn(msg);
        }
    }

    public class CLogColor
    {
        public static string Yellow = "#ff7f00";
        public static string Red = "#ff0000";
    }
}
