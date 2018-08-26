/*******************************************************
 * Class  : ConsoleLogger 
 * Date   : 2014/1/31
 * Author : Alvin
 * ******************************************************/

using UnityEngine;
using System.Collections.Generic;

public class ConsoleLogger : MonoBehaviour
{
    private struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    public KeyCode toggleKey = KeyCode.Tab;

    private List<Log> logs = new List<Log>();
    private Vector2 scrollPosition;
    public bool show;
    private bool collapse;


    private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    };

    private const int margin = 20;

    private Rect windowRect = new Rect(margin, margin * 2, Screen.width * .9f, Screen.height - (margin * 3));
    private Rect titleBarRect = new Rect(0, 0, 10000, 45);
    private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    private LogType? currentLogType = null;

    private void OnEnable()
    {
#if UNITY_4_6
		Application.RegisterLogCallback(HandleLog);
#else
        Application.logMessageReceived += HandleLog;
#endif
    }

    private void OnDisable()
    {
#if !UNITY_4_6
        Application.logMessageReceived -= HandleLog;
#endif
    }

    private Rect m_sLeftTopRect;
    private Rect m_sLeftBottomRect;
    private Rect m_sRightTopRect;
    private Rect m_sRightBottomRect;
    private int m_nClickIndex;
    private void Start()
    {
        m_sLeftTopRect = new Rect(0, Screen.height - 50, 50, 50);
        m_sLeftBottomRect = new Rect(0, 0, 50, 50);
        m_sRightTopRect = new Rect(Screen.width - 50, Screen.height - 50, 50, 50);
        m_sRightBottomRect = new Rect(Screen.width - 50, 0, 50, 50);
    }

    private float prevToggleTime = 0;

    private void Update()
    {

        //在移动设备上通过五指同时触摸来激活
#if !UNITY_STANDALONE && !UNITY_EDITOR
        if (Input.touches.Length >= 5 
            && (Time.time - prevToggleTime > .5f)) {
                prevToggleTime = Time.time;
                ToggleShowDebug();
        }
        else
        {
            if(Input.touches.Length > 0)
            {
                var position = Input.GetTouch(0).position;
                if (m_sLeftTopRect.Contains(position))
                {
                    m_nClickIndex = 1;
                }
                if(m_nClickIndex == 1 && m_sLeftBottomRect.Contains(position))
                {
                    m_nClickIndex = 2;
                }
                if (m_nClickIndex == 2 && m_sRightTopRect.Contains(position))
                {
                    m_nClickIndex = 3;
                }
                if (m_nClickIndex == 3 && m_sRightBottomRect.Contains(position))
                {
                    ToggleShowDebug();
                }
            }
        }
#endif
        if (Input.GetKeyDown(toggleKey)) {
            ToggleShowDebug();
        }
    }

    public void ToggleShowDebug()
    {
        show = !show;
    }

    private void OnGUI()
    {
        if (!show) return;
        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
    }

    private void ConsoleWindow(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < logs.Count; i++) {
            var log = logs[i];

            if (collapse) {
                var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                if (messageSameAsPrevious) {
                    continue;
                }
            }

            GUI.contentColor = logTypeColors[log.type];
            GUILayout.Label(log.message);
        }

        GUILayout.EndScrollView();

        GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel)) {
            logs.Clear();
        }

        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        GUI.DragWindow(titleBarRect);
    }

    private void HandleLog(string message, string stackTrace, LogType type)
    {
        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
    }
}