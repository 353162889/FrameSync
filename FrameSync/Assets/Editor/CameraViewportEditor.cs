using Framework;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraViewport))]
public class CameraViewportEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraViewport cameraViewport = (CameraViewport)target;
        TSRect rect = cameraViewport.mRect;
        var unityRect = rect.ToUnityRect();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rect");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.RectField(unityRect);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("计算viewport"))
        {
            var pos = cameraViewport.transform.position;
            var camera = cameraViewport.gameObject.GetComponent<Camera>();
            if (camera == null)
            {
                camera = cameraViewport.GetComponentInChildren<Camera>();
            }
            if(camera == null)
            {
                Debug.LogError(cameraViewport.gameObject.name + "下找不到相机组件");
                return;
            }

            //float radio = 750f / 1334f;
            //float radio = 1 / 2f;
            float radio = 10 / 16f;//最大的手机宽度比
            float height;
            float width;
            if (camera.orthographic)
            {
                height = camera.orthographicSize * 2;
                width = height * radio;
            }
            else
            {
                float len = (camera.transform.position - pos).y;
                height = len * Mathf.Tan(camera.fieldOfView / 2f * Mathf.Deg2Rad) * 2;
                width = height * radio;
            }
            FP x = FP.FromFloat(pos.x - width / 2);
            FP y = FP.FromFloat(pos.z - height / 2);
            FP fpWidth = FP.FromFloat(width);
            FP fpHeight = FP.FromFloat(height);
            cameraViewport.mRect = new TSRect(x, y, fpWidth, fpHeight);
            EditorUtility.SetDirty(cameraViewport.gameObject);
            Debug.Log("计算成功");
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

    }
}
