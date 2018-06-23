using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

public class TestHangPoint : MonoBehaviour
{
    private HangPoint m_cHangPoint;
    private GameObject go1;
    private GameObject go2;
    private GameObject go3;
    void Start()
    {
        gameObject.AddComponentOnce<ResourceSys>();
        ResourceSys.Instance.Init(true, "Assets/ResourceEx");
        HangPointCfgSys.Instance.LoadResCfgs(null);
        m_cHangPoint = gameObject.AddComponent<HangPoint>();
        m_cHangPoint.Init("Prefab/Test/HangPointTest.prefab");
        go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }
    void Update()
    {
        TSVector pos;
        TSVector forward;
        Transform t;
        t = m_cHangPoint.GetHangPoint("HangPoint1", TSVector.FromUnitVector3(transform.position), TSVector.FromUnitVector3(transform.forward), out pos, out forward);

        if (t != null)
        {
            t.gameObject.AddChildToParent(go1, "HangPoint1Child", false);
        }
        else
        {
            go1.transform.position = pos.ToUnityVector3();
            go1.transform.forward = forward.ToUnityVector3();
        }

        t = m_cHangPoint.GetHangPoint("HangPoint2", TSVector.FromUnitVector3(transform.position), TSVector.FromUnitVector3(transform.forward), out pos, out forward);
        if (t != null)
        {
            t.gameObject.AddChildToParent(go2, "HangPoint2Child", false);
        }
        else
        {
            go2.transform.position = pos.ToUnityVector3();
            go2.transform.forward = forward.ToUnityVector3();
        }

        t = m_cHangPoint.GetHangPoint("HangPoint3", TSVector.FromUnitVector3(transform.position), TSVector.FromUnitVector3(transform.forward), out pos, out forward);
        if (t != null)
        {
            t.gameObject.AddChildToParent(go3, "HangPoint3Child", false);
        }
        else
        {
            go3.transform.position = pos.ToUnityVector3();
            go3.transform.forward = forward.ToUnityVector3();
        }
    }
}
