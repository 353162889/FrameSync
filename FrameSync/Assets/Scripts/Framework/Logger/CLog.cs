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
        private static bool m_bDebug = true;
        public static void Init() { }

        public static void Log(string msg, string color = "")
        {
            if (!m_bDebug) return;
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

        public static void LogArgs(params object[] args)
        {
            if (!m_bDebug) return;
            if (args.Length <= 0) return;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].ToString());
                if(i < args.Length - 1)
                {
                    sb.Append("\t");
                }
            }
            Debug.Log(sb.ToString());
        }

        public static void LogColorArgs(string color,params object[] args)
        {
            if (!m_bDebug) return;
            if (args.Length <= 0) return;
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=" + color + ">");
            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].ToString());
                if (i < args.Length - 1)
                {
                    sb.Append("\t");
                }
            }
            sb.Append("</color>");
            Debug.Log(sb.ToString());
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
        public static string Green = "#00ff00";
        public static string Blue = "#0000ff";
    }
}
