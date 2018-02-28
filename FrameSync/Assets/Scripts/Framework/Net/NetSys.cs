using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public delegate void MsgCallback(object netObj);
    public class NetSys : SingletonMonoBehaviour<NetSys>
    {
        private NetChannel[] m_arrChannel;
        protected override void Init()
        {
            int len = Enum.GetValues(typeof(NetChannelType)).Length;
            m_arrChannel = new NetChannel[len];
            for (int i = 0; i < len; i++)
            {
                CreateChannel((NetChannelType)i);
            }
          
        }

        protected void CreateChannel(NetChannelType channel)
        {
            m_arrChannel[(int)channel] = new NetChannel(channel,new TCPSocketClient());
        }

        protected NetChannel GetChannel(NetChannelType type)
        {
            return m_arrChannel[(int)type];
        }

        //监听回复消息
        public void SendMsg(NetChannelType channel,short sendOpcode,short receiveOpcode, object data,MsgCallback callback)
        {
            AddMsgCallback(channel, receiveOpcode, callback);
            SendMsg(channel, sendOpcode, data);
        }

        //不监听回复消息
        public void SendMsg(NetChannelType channel,short sendOpcode,object data)
        {
            NetSendData sendData;
            sendData.sendOpcode = sendOpcode;
            sendData.data = data;
            NetChannel netChannel = GetChannel(channel);
            if (netChannel == null) return;
            netChannel.SendMsg(sendData);
        }

        public void AddMsgCallback(NetChannelType channel,short receiveOpcode,MsgCallback callback,bool once = false)
        {
            if (callback != null)
            {
                NetChannel netChannel = GetChannel(channel);
                if (netChannel == null) return;
                netChannel.AddMsgCallback(receiveOpcode, callback, once);
            }
        }

        public void RemoveMsgCallback(NetChannelType channel,short receiveOpcode,MsgCallback callback)
        {
            if(callback != null)
            {
                NetChannel netChannel = GetChannel(channel);
                if (netChannel == null) return;
                netChannel.RemoveMsgCallback(receiveOpcode, callback);
            }
        }

        public bool BeginConnect(NetChannelType channel,string ip,int port,Action<bool,NetChannelType> callback = null)
        {
            NetChannel netChannel = GetChannel(channel);
            if (netChannel == null) return false;
            return netChannel.BeginConnect(ip, port, callback);
        }

        public void DisConnect(NetChannelType channel)
        {
            NetChannel netChannel = GetChannel(channel);
            if (netChannel == null) return;
            netChannel.DisConnect();
        }

        public bool RunFrameData(NetChannelType channel, int frameIndex)
        {
            NetChannel netChannel = GetChannel(channel);
            if (netChannel == null) return false;
            return netChannel.RunFrameData(frameIndex);
        }

        void Update()
        {
            for (int i = 0; i < m_arrChannel.Length; i++)
            {
                try
                {
                    if (m_arrChannel[i] != null)
                    {
                        m_arrChannel[i].OnUpdate();
                    }
                }
                catch (Exception e)
                {
                    CLog.LogError(e.Message+"\n"+e.StackTrace);
                }
            }
        }
        
        void OnApplicationQuit()
        {
            for (int i = 0; i < m_arrChannel.Length; i++)
            {
                if (m_arrChannel[i] != null)
                {
                    m_arrChannel[i].Dispose();
                }
                m_arrChannel[i] = null;
            }
        }
    }
}