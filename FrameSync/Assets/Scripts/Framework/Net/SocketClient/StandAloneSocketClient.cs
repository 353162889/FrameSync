﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class StandAloneSocketClient : SocketClient
    {
        private static float FrameSpaceTime = 1f / 20;
        private Queue<NetSendData> m_queueSend;
        private Queue<NetSendData> m_queueFrameSend;
        private Queue<NetRecvData> m_queueRecv;
        private float m_fTime;
        private int m_nFrameIndex;
        private bool m_bStart;
        public StandAloneSocketClient()
        {
            m_queueSend = new Queue<NetSendData>();
            m_queueFrameSend = new Queue<NetSendData>();
            m_queueRecv = new Queue<NetRecvData>();
            m_nFrameIndex = 0;
            m_bStart = false;
        }
        public override int RecvNetData(Queue<NetRecvData> queue)
        {
            int count = m_queueRecv.Count;
            while(m_queueRecv.Count > 0)
            {
                queue.Enqueue(m_queueRecv.Dequeue());
            }
            return count;
        }

        public override void SendNetData(NetSendData data)
        {
            if(data.sendOpcode < 10000)
            {
                m_queueSend.Enqueue(data);
            }
            else
            {
                if (m_bStart)
                {
                    m_queueFrameSend.Enqueue(data);
                }
            }
        }

        public void StartFrameSync()
        {
            m_bStart = true;
            m_fTime = 0;
            m_nFrameIndex = 0;
        }

        public void StopFrameSync()
        {
            m_bStart = false;
            m_fTime = 0;
            m_nFrameIndex = 0;
        }

        protected override bool BeginConnect(string ip, int port)
        {
            m_fTime = 0 ;
            m_nFrameIndex = 0;
            return true;
        }

        protected override SocketClientStatus SocketConnectingStatus()
        {
            return SocketClientStatus.Connected;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if(m_cHeartBeatInfo != null)
            {
                if(m_cHeartBeatInfo.sendHandler != null)
                {
                    NetSendData sendData;
                    if(m_cHeartBeatInfo.sendHandler.Invoke(out sendData))
                    {
                        SendNetData(sendData);
                    }
                }
            }
            //一般消息处理
            while(m_queueSend.Count > 0)
            {
                var sendData = m_queueSend.Dequeue();
                if(m_cHeartBeatInfo != null && sendData.sendOpcode == m_cHeartBeatInfo.sReceiveHeartBeatOpcode)
                {
                    if(m_cHeartBeatInfo.receiveHandler != null)
                    {
                        if(!m_cHeartBeatInfo.receiveHandler.Invoke())
                        {
                            continue;
                        }
                    }
                }
                NetRecvData recvData = new NetRecvData();
                recvData.recvOpcode = sendData.sendOpcode;
                recvData.data = sendData.data;
                recvData.len = 0;//这里的长度直接填0（不需要反序列化）
                m_queueRecv.Enqueue(recvData);
            }
            if (m_bStart)
            {
                //每隔50毫秒一次帧同步
                m_fTime += Time.deltaTime;
                if (m_fTime >= FrameSpaceTime)
                {
                    m_fTime -= FrameSpaceTime;
                    m_nFrameIndex++;
                    //获取到所有的帧包
                    NetRecvData recvFrameData = new NetRecvData();
                    recvFrameData.data = m_nFrameIndex;
                    recvFrameData.recvOpcode = 0;
                    int len = m_queueFrameSend.Count;
                    if (len > 255)
                    {
                        len = 255;
                    }
                    recvFrameData.len = (short)len;
                    m_queueRecv.Enqueue(recvFrameData);
                    for (int i = 0; i < len; i++)
                    {
                        NetRecvData recvData = new NetRecvData();
                        var sendData = m_queueFrameSend.Dequeue();
                        recvData.recvOpcode = sendData.sendOpcode;
                        recvData.data = sendData.data;
                        recvData.len = 0;//这里的长度直接填0（不需要反序列化）
                        m_queueRecv.Enqueue(recvData);
                    }
                }
            }
        }
    }
}
