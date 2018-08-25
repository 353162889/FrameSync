using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Framework
{
    public enum NetChannelType
    {
        Game = 0
    }

    public enum NetChannelModeType
    {
        Tcp,
        StandAlone,
    }


    /// <summary>
    /// 主要处理一个网络连接，网络事件处理（包括回调）,心跳，重连
    /// </summary>
    public class NetChannel
    {
        public struct MsgDispatcherCallback : IDynamicDispatcherObj
        {
            public bool once;
            public MsgCallback callback;

            public bool OnDispatcher(object obj)
            {
                if (callback != null)
                {
                    callback(obj);
                    return !once;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                MsgDispatcherCallback other = (MsgDispatcherCallback)obj;
                return callback == other.callback;
            }

            public override int GetHashCode()
            {
                return this.callback.GetHashCode();
            }

            public bool EqualOther(IDynamicDispatcherObj dispatcherObj)
            {
                return this.Equals(dispatcherObj);
            }

            public static bool operator ==(MsgDispatcherCallback lhs, MsgDispatcherCallback rhs)
            {
                return lhs.Equals(rhs);
            }

            public static bool operator !=(MsgDispatcherCallback lhs, MsgDispatcherCallback rhs)
            {
                return !(lhs == rhs);
            }
        }

        private NetChannelType m_eChannelType;
        private Queue<NetRecvData> m_queueRecvData;
        private SocketClient m_cSocketClient;
        private Action<bool, NetChannelType> m_cCallback;
        private Dictionary<short, List<IServerMsg>> m_dicServerMsg;
        public EDynamicDispatcher m_cMsgDispatcher;
        private NetFrameData m_cFrameData;

        public SocketClientStatus Status {
            get { return m_cSocketClient.Status; }
        }
        public NetChannel(NetChannelType type,SocketClient socketClient)
        {
            m_eChannelType = type;
            m_queueRecvData = new Queue<NetRecvData>();
            m_cSocketClient = socketClient;
            m_cSocketClient.OnDisConnect += OnDisConnect;
            m_dicServerMsg = new Dictionary<short, List<IServerMsg>>();
            m_cMsgDispatcher = new EDynamicDispatcher();
            m_cFrameData = new NetFrameData();
            LoadNetMessageEvent();
        }

        private void LoadNetMessageEvent()
        {
            m_dicServerMsg.Clear();
            Assembly assembly = Assembly.GetAssembly(typeof(NetSys));
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                object[] arr = type.GetCustomAttributes(typeof(NetMsgAttribute), false);
                if(arr.Length == 0)
                {
                    continue;
                }
                Type baseType = type.BaseType;
                if (!baseType.IsGenericType) continue;
                Type dataType = baseType.GetGenericArguments()[0];
                string opcodeName = dataType.Name.Replace("_Data", "");
                short opcode = (short)(int)Enum.Parse(typeof(Proto.PacketOpcode), opcodeName);
                var msg = Activator.CreateInstance(type) as IServerMsg;
                List<IServerMsg> lst;
                m_dicServerMsg.TryGetValue(opcode, out lst);
                if(lst == null)
                {
                    lst = new List<IServerMsg>();
                    m_dicServerMsg.Add(opcode, lst);
                }
                lst.Add(msg);
            }
        }

        //系统断开连接
        private void OnDisConnect()
        {
            //网络自动断开，可以处理断线重连
            CLog.Log("系统断开连接", CLogColor.Red);
        }

        public void SendMsg(NetSendData data)
        {
            if (m_cSocketClient.Status == SocketClientStatus.Connected)
            {
                m_cSocketClient.SendNetData(data);
            }
        }

        public void AddMsgCallback(short receiveOpcode, MsgCallback callback,bool once)
        {
            MsgDispatcherCallback cc = new MsgDispatcherCallback();
            cc.once = once;
            cc.callback = callback;
            m_cMsgDispatcher.AddDispatcherObj((int)receiveOpcode, cc);
        }

        public void RemoveMsgCallback(short receiveOpcode,MsgCallback callback)
        {
            MsgDispatcherCallback cc = new MsgDispatcherCallback();
            cc.callback = callback;
            m_cMsgDispatcher.RemoveDispatcherObj((int)receiveOpcode, cc);
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
            if(m_cMsgDispatcher != null)
            {
                m_cMsgDispatcher.Clear();
                m_cMsgDispatcher = null;
            }
            if(m_cFrameData != null)
            {
                m_cFrameData.Clear();
                m_cFrameData = null;
            }
        }

        public void OnUpdate()
        {
            //处理网络事件
            if (m_cSocketClient != null && m_cSocketClient.Status != SocketClientStatus.DisConnect)
            {
                m_cSocketClient.OnUpdate();
                if (m_cSocketClient.Status == SocketClientStatus.Connected)
                {
                    m_cSocketClient.RecvNetData(m_queueRecvData);
                    //限制每帧处理包的最大个数可以在这里加
                    while (m_queueRecvData.Count > 0)
                    {
                        NetRecvData recvData = m_queueRecvData.Dequeue();
                        //如果当前非帧包，直接运行
                        if (!m_cFrameData.AddRecvData(recvData))
                        {
                            HandleRecvData(recvData);
                        }
                    }
                }
            }
        }

        private void HandleRecvData(NetRecvData recvData)
        {
            List<IServerMsg> lst;
            m_dicServerMsg.TryGetValue(recvData.recvOpcode, out lst);
            if(lst != null)
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    lst[i].HandleMsg(recvData.data);
                }
            }
            m_cMsgDispatcher.Dispatch(recvData.recvOpcode, recvData.data);
        }

        public bool CanRunFrameData(int frameIndex)
        {
            if (Status != SocketClientStatus.Connected) return false;
            if (frameIndex >= m_cFrameData.recvFrameIndex) return false;
            return true;
        }

        public bool RunFrameData(int frameIndex)
        {
            if (Status != SocketClientStatus.Connected) return false;
            if (frameIndex >= m_cFrameData.recvFrameIndex) return false;
            var lst = m_cFrameData.GetFrameData(frameIndex);
            if(lst != null)
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    try
                    {
                        HandleRecvData(lst[i]);
                    }
                    catch(Exception e)
                    {
                        CLog.LogError("handle frame data error,opcode="+lst[i].recvOpcode + ",\n"+e.ToString());
                    }
                }
            }
            m_cFrameData.RemoveFrameData(frameIndex);
            return true;
        }
    }
}
