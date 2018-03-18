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
        NetSys.Instance.CreateChannel(NetChannelType.Game, NetChannelModeType.StandAlone);
        FrameSyncSys.Instance.StartRun();

        NetSys.Instance.BeginConnect(NetChannelType.Game, "127.0.0.1", 8080, OnCallback);
        NetSys.Instance.AddMsgCallback(NetChannelType.Game, (short)Proto.PacketOpcode.Msg_Test, OnCallback, false);
	}

    private void OnCallback(object netObj)
    {
        var data = netObj as Proto.Msg_Test_Data;
        CLog.Log("recv:"+data.msg);
    }

    private void OnCallback(bool succ, NetChannelType channel)
    {
        CLog.Log("返回连接服务器结果:"+succ);
    }

    // Update is called once per frame
    void Update () {
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "发送一般测试包"))
        {
            Proto.Msg_Test_Data data = new Proto.Msg_Test_Data();
            data.msg = "发送测试包";
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)Proto.PacketOpcode.Msg_Test, data);
        }
        if (GUI.Button(new Rect(200, 0, 200, 100), "进入或创建房间"))
        {
            Proto.C2S_JoinOrCreateRoom_Data data = new Proto.C2S_JoinOrCreateRoom_Data();
            data.roomId = -1;
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)Proto.PacketOpcode.C2S_JoinOrCreateRoom, data);
        }
        if(GUI.Button(new Rect(400,0,200,100),"发送帧同步包"))
        {
            Proto.Frame_Msg_Test_Data data = new Proto.Frame_Msg_Test_Data();
            data.msg = "发送帧同步包";
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)Proto.PacketOpcode.Frame_Msg_Test,data);
        }
    }
}
