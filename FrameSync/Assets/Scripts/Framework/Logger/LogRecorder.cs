using System;
using System.IO;
using UnityEngine;

namespace Framework
{
    public class LogRecorder
    {
        private FileStream _file;
        private string _logFilePath;
        private BinaryWriter _fileWriter;
        private static string DataPath = Application.dataPath;
        private static string PersistanceDataPath = Application.persistentDataPath;


        public string LogFilePath
        {
            get
            {
                if (_logFilePath == null)
                {
                    if ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.WindowsEditor) ||
                        (Application.platform == RuntimePlatform.OSXPlayer) || (Application.platform == RuntimePlatform.OSXEditor))
                    {
                        _logFilePath = DataPath + "/../logInfo.log";

                    }
                    else 
                    {
                        _logFilePath = PersistanceDataPath + "/logInfo.log";
                    }
                }
                return _logFilePath;
            }
        }

        public LogRecorder()
        {
            this._file = new FileStream(LogFilePath, FileMode.Create);
            this._fileWriter = new BinaryWriter(this._file);
        }

        ~LogRecorder()
        {
            if ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.WindowsEditor) ||
                (Application.platform == RuntimePlatform.OSXPlayer) || (Application.platform == RuntimePlatform.OSXEditor))
            {
                if (this._fileWriter != null)
                {
                    this._fileWriter.Close();
                    this._fileWriter = null;
                }
                if (this._file != null)
                {
                    this._file.Close();
                    this._file = null;
                }
            }
        }

        public void Log(object msg)
        {
            string str = (msg == null) ? "null" : msg.ToString();
            string[] formatStrs = new string[] { "[INFO：", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "]", str, "\r\n" };
            this._fileWriter.Write(string.Concat(formatStrs).ToCharArray());
            this._fileWriter.Flush();
        }

        public void LogWarn(object msg)
        {
            string str = (msg == null) ? "null" : msg.ToString();
            string[] formatStrs = new string[] { "[WARN：", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "]", str, "\r\n" };
            this._fileWriter.Write(string.Concat(formatStrs).ToCharArray());
            this._fileWriter.Flush();
        }

        public void LogError(object msg)
        {
            string str = (msg == null) ? "null" : msg.ToString();
            string[] formatStrs = new string[] { "[ERROR：", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "]", str, "\r\n" };
            this._fileWriter.Write(string.Concat(formatStrs).ToCharArray());
            this._fileWriter.Flush();
        }
    }
}
