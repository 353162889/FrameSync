using Framework;
using Game;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(CameraViewport))]
public class CameraViewportEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraViewport cameraViewport = (CameraViewport)target;
        TSRect tsRect = cameraViewport.mRect;
        var unityRect = tsRect.ToUnityRect();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rect");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.RectField(unityRect);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("从ResScene中读取视野宽高，并调整相机视野"))
        {
            ResCfgSys.Instance.LoadResCfgs("Assets/ResourceEx/Config/Data");
            var lst = ResCfgSys.Instance.GetCfgLst<ResScene>();
            ResScene resInfo = null;
            for (int i = 0; i < lst.Count; i++)
            {
                if(lst[i].name == EditorSceneManager.GetActiveScene().name)
                {
                    resInfo = lst[i];
                    break;
                }
            }

            if(resInfo == null)
            {
                EditorUtility.DisplayDialog("提示","当前场景名:"+ EditorSceneManager.GetActiveScene().name +"在ResScene中找不到相应的配置","确定");
                return;
            }
            tsRect = new TSRect(-resInfo.view_width / 2, -resInfo.view_height / 2, resInfo.view_width, resInfo.view_height);
            cameraViewport.mRect = tsRect;


            var camera = cameraViewport.gameObject.GetComponent<Camera>();
            if (camera == null)
            {
                camera = cameraViewport.GetComponentInChildren<Camera>();
            }
            if (camera == null)
            {
                Debug.LogError(cameraViewport.gameObject.name + "下找不到相机组件");
                return;
            }
            var rect = tsRect.ToUnityRect();
            if (camera.orthographic)
            {
                camera.orthographicSize = rect.height / 2;
            }
            else
            {
                float len = (camera.transform.position - Vector3.zero).magnitude;
                float angle = Mathf.Rad2Deg * Mathf.Atan2(rect.height / 2, len);
                camera.fieldOfView = angle * 2;
            }

            //var pos = cameraViewport.transform.position;
            //var camera = cameraViewport.gameObject.GetComponent<Camera>();
            //if (camera == null)
            //{
            //    camera = cameraViewport.GetComponentInChildren<Camera>();
            //}
            //if (camera == null)
            //{
            //    Debug.LogError(cameraViewport.gameObject.name + "下找不到相机组件");
            //    return;
            //}

            ////float radio = 750f / 1334f;
            ////float radio = 1 / 2f;
            //float radio = 10 / 16f;//最大的手机宽度比
            //float height;
            //float width;
            //if (camera.orthographic)
            //{
            //    height = camera.orthographicSize * 2;
            //    width = height * radio;
            //}
            //else
            //{
            //    float len = (camera.transform.position - pos).y;
            //    height = len * Mathf.Tan(camera.fieldOfView / 2f * Mathf.Deg2Rad) * 2;
            //    width = height * radio;
            //}
            //FP x = FP.FromFloat(pos.x - width / 2);
            //FP y = FP.FromFloat(pos.z - height / 2);
            //FP fpWidth = FP.FromFloat(width);
            //FP fpHeight = FP.FromFloat(height);
            //cameraViewport.mRect = new TSRect(x, y, fpWidth, fpHeight);
            EditorUtility.SetDirty(cameraViewport.gameObject);
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

    }
}
