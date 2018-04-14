using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security;
using Mono.Xml;
using GameData;

public class TestXml : MonoBehaviour
{
    void Start()
    {
        gameObject.AddComponent<ResourceSys>();
        ResourceSys.Instance.Init(true, "Assets/ResourceEx");
        //ResourceSys.Instance.GetResource("ResTest.xml",OnSucc);
        ResCfgSys.Instance.LoadResCfgs("",OnFinish);
    }

    private void OnFinish()
    {
        //var lst = ResCfgSys.Instance.GetCfgLst<ResTest>();
        //for (int i = 0; i < lst.Count; i++)
        //{
        //    CLog.LogArgs(lst[i].id,lst[i].point,lst[i].desc);
        //}
        //var lst1 = ResCfgSys.Instance.GetCfgLst<ResTest1>();
        //for (int i = 0; i < lst1.Count; i++)
        //{
        //    CLog.LogArgs(lst1[i].id, lst1[i].point, lst1[i].desc);
        //}
    }

    //private void OnSucc(Resource res)
    //{
    //    SecurityParser parser = new SecurityParser();
    //    parser.LoadXml(res.GetText());
    //    SecurityElement element = parser.ToXml();
    //    foreach (SecurityElement node in element.Children)
    //    {
    //        var t = new ResTest(node);
    //        CLog.Log("aa");
    //    } 
    //    CLog.Log(res.GetText());
    //}
}
