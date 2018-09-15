using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TestCheck2D : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform Box;
    public Transform Box1;
    public Transform Sphere;
    private TSVector2 To(Vector3 pos)
    {
        return new TSVector2(FP.FromFloat(pos.x),FP.FromFloat(pos.z));
    }
    void Start()
    {
        //Debug.Log(TSMath.Cos(90 * FP.Deg2Rad));
    }

    void Update()
    {
        //var offset = To(B.position) - To(A.position);
        //FP result = TSCheck2D.CheckAabbAndLine(To(Box.position), FP.FromFloat(Box.localScale.x / 2f), FP.FromFloat(Box.localScale.z / 2f), To(A.position), offset.normalized, offset.magnitude);
        //if (result >= 0)
        //{
        //    if (go == null)
        //    {
        //        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //    }
        //    TSVector2 resultOffset = offset.normalized * result;
        //    go.transform.position = A.position + new Vector3(resultOffset.x.AsFloat(), 0, resultOffset.y.AsFloat());
        //}
        //else
        //{
        //    if (go != null)
        //    {
        //        GameObject.Destroy(go);
        //        go = null;
        //    }
        //}

        //var offset = To(B.position) - To(A.position);
        //if (TSCheck2D.CheckRectangleAndLine(To(Box.position), To(Box.forward), FP.FromFloat(Box.localScale.x / 2f), FP.FromFloat(Box.localScale.z / 2f), To(A.position), ref offset))
        //{
        //    if (go == null)
        //    {
        //        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //    }
        //    go.transform.position = A.position + new Vector3(offset.x.AsFloat(), 0, offset.y.AsFloat());
        //}
        //else
        //{
        //    if (go != null)
        //    {
        //        GameObject.Destroy(go);
        //        go = null;
        //    }
        //}

        //if (TSCheck2D.CheckRectangleAndPos(To(Box.position), To(Box.forward), FP.FromFloat(Box.localScale.x / 2f), FP.FromFloat(Box.localScale.z / 2f), To(A.position)))
        //{
        //    if (go == null)
        //    {
        //        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //    }
        //    go.transform.position = A.position;
        //}
        //else
        //{
        //    if (go != null)
        //    {
        //        GameObject.Destroy(go);
        //        go = null;
        //    }
        //}

        //FP dis = TSCheck2D.DistanceFromPointToLine(To(Box.position), To(A.position), To(B.position));
        //Debug.Log(dis);

        //var offset = To(B.position) - To(A.position);
        //TSVector2 crossPoint;
        //if (TSCheck2D.CheckCicleAndLine(To(A.position), offset, To(Sphere.position), FP.FromFloat(Sphere.localScale.x / 2), out crossPoint))
        //{
        //    if (go == null)
        //    {
        //        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //    }
        //    go.transform.position = new Vector3(crossPoint.x.AsFloat(), 0, crossPoint.y.AsFloat());
        //}
        //else
        //{
        //    if (go != null)
        //    {
        //        GameObject.Destroy(go);
        //        go = null;
        //    }
        //}

        //if (TSCheck2D.CheckRectangleAndCircle(To(Box.position), To(Box.forward), FP.FromFloat(Box.localScale.x / 2f), FP.FromFloat(Box.localScale.z / 2f),To(Sphere.position),FP.FromFloat(Sphere.localScale.x / 2)))
        //{
        //    if (go == null)
        //    {
        //        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //    }
        //    go.transform.position = new Vector3(2,0,0);
        //}
        //else
        //{
        //    if (go != null)
        //    {
        //        GameObject.Destroy(go);
        //        go = null;
        //    }
        //}

        if(TSCheck2D.CheckRectangleAndRectangle(To(Box.position),To(Box.forward), FP.FromFloat(Box.localScale.x / 2f), FP.FromFloat(Box.localScale.z / 2f), 
            To(Box1.position), To(Box1.forward), FP.FromFloat(Box1.localScale.x / 2f), FP.FromFloat(Box1.localScale.z / 2f)))
        {
            if (go == null)
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            go.transform.position = new Vector3(2, 0, 0);
            Debug.Log("碰撞");
        }
        else
        {
            if (go != null)
            {
                GameObject.Destroy(go);
                go = null;
            }
            Debug.Log("非碰撞");
        }
    }

    private GameObject go;

    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,50),"计算碰撞"))
        {
            var offset = To(B.position) - To(A.position);
            if(TSCheck2D.CheckRectangleAndLine(To(Box.position), To(Box.forward), FP.FromFloat(Box.localScale.x / 2f), FP.FromFloat(Box.localScale.z/2f), To(A.position), ref offset))
            {
                Debug.Log("碰撞成功");
                if (go == null)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                }
                go.transform.position = A.position + new Vector3(offset.x.AsFloat(),0,offset.y.AsFloat());
            }
            else
            {
                Debug.Log("碰撞失败");
            }
        }

        if (GUI.Button(new Rect(100, 0, 100, 50), "计算AABB碰撞"))
        {
            var offset = To(B.position) - To(A.position);
            FP result = TSCheck2D.CheckAabbAndLine(To(Box.position), FP.FromFloat(Box.localScale.x / 2f), FP.FromFloat(Box.localScale.z / 2f), To(A.position), offset.normalized, offset.magnitude);
            if(result >= 0)
            {
                Debug.Log("碰撞成功");
                if (go == null)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                TSVector2 resultOffset = offset.normalized * result;
                go.transform.position = A.position + new Vector3(resultOffset.x.AsFloat(), 0, resultOffset.y.AsFloat());
            }
            else
            {
                Debug.Log("碰撞失败");
                if (go != null)
                {
                    GameObject.Destroy(go);
                    go = null;
                }
            }
        }
    }
}
