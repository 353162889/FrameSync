using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public enum NetChannelType
    {
        Game = 0
    }

    public struct NetSendData
    {
        public short sendOpcode;
        public object data;
    }

    public struct NetRecvData
    {
        public short recvOpcode;
        public object data;
    }

    /// <summary>
    /// 主要处理一个网络连接，网络事件处理（包括回调）,心跳，重连
    /// </summary>
    public class NetChannel
    {
        private NetChannelType m_eChannelType;
        private Queue<NetRecvData> m_queueRecvData;
        private SocketClient m_cSocketClient;
        private Action<bool, NetChannelType> m_cCallback;

        public SocketClientStatus Status {
            get { return m_cSocketClient.Status; }
        }
        public NetChannel(NetChannelType type,SocketClient socketClient)
        {
            m_eChannelType = type;
            m_queueRecvData = new Queue<NetRecvData>();
            m_cSocketClient = socketClient;
            m_cSocketClient.OnDisConnect += OnDisConnect;
        }

        //系统断开连接
        private void OnDisConnect()
        {
            //网络自动断开，可以处理断线重连
        }

        public void SendMsg(NetSendData data)
        {
            m_cSocketClient.SendNetData(data);
        }

        public void AddMsgCallback(short receiveOpcode, MsgCallback callback)
        {

        }

        public bool BeginConnect(string ip, int port, Action<bool, NetChannelType> callback)
        {
            m_cCallback = callback;
            m_cSocketClient.BeginConnect(ip, port, OnConnect);
            return true;
        }

        private void OnConnect(bool succ)
        {
            if(m_cCallback != null)
            {
                m_cCallback(succ, m_eChannelType);
                m_cCallback = null;
            }
        }

        //手动断开
        public void DisConnect()
        {
            //手动调用断开连接不执行回调方法
            m_cCallback = null;
            m_cSocketClient.DisConnect();
        }

        public void Dispose()
        {
            m_cSocketClient.Dispose();
            m_cSocketClient = null;
        }

        public void OnUpdate()
        {
            //处理网络事件
            if (m_cSocketClient != null)
            {
                m_cSocketClient.OnUpdate();
                int count = m_cSocketClient.RecvNetData(m_queueRecvData);
                while(m_queueRecvData.Count > 0)
                {
                    var recvData = m_queueRecvData.Dequeue();
                    //处理包
                }
            }
        }
    }
}
