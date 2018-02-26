using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;

public class TestNet : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.AddComponentOnce<NetSys>();
        gameObject.AddComponentOnce<FrameSyncSys>();

        NetSys.Instance.BeginConnect(NetChannelType.Game, "", 100, OnCallback);
	}

    private void OnCallback(bool succ, NetChannelType channel)
    {
        CLog.Log("连接服务器成功");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
